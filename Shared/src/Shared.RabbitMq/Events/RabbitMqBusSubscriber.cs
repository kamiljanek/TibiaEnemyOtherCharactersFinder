using System.Collections.Concurrent;
using System.Text;
using EkoProTech.Shared.RabbitMQ.EventBus;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared.RabbitMQ.Configuration;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.EventBus;

namespace Shared.RabbitMQ.Events;

public class RabbitMqBusSubscriber : IEventBusSubscriber
{
    private static readonly ConcurrentDictionary<string, IModel> Channels = new();

    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly ILogger<RabbitMqBusSubscriber> _logger;
    private readonly MessageSerializer _serializer;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConnection _connection;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly RabbitMqSection _options;

    public RabbitMqBusSubscriber(IRabbitMqConventionProvider conventionProvider,
        SubscriberConnection connection,
        ILogger<RabbitMqBusSubscriber> logger,
        MessageSerializer serializer,
        IServiceProvider serviceProvider,
        IOptions<RabbitMqSection> options)
    {
        _options = options.Value;
        _conventionProvider = conventionProvider;
        _logger = logger;
        _serializer = serializer;
        _serviceProvider = serviceProvider;
        _connection = connection.Connection;
        _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(_options.Retries, _ => TimeSpan.FromSeconds(_options.RetryInterval));

    }
    public void Subscribe<T, TResult>(EventBusHandler<T, TResult> handler) where T : class
    {
        QueueBinding queueBinding = _conventionProvider.GetForType(typeof(T));

        if (Channels.ContainsKey(queueBinding.Key))
        {
            return;
        }

        IModel channel = _connection.CreateModel();
        if (!Channels.TryAdd(queueBinding.Key, channel))
        {
            channel.Dispose();
            return;
        }

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (_, args) =>
        {
            try
            {
                var messageId = args.BasicProperties.MessageId;
                var timestamp = args.BasicProperties.Timestamp.UnixTime;

                _logger.LogInformation("Received message: '{MessageId}' with timestamp '{timestamp}'", messageId,
                    timestamp);

                if (await handler.ShouldHandleAsync(_serviceProvider, messageId))
                {
                    var payload = Encoding.UTF8.GetString(args.Body.Span);
                    var message = _serializer.Deserialize<T>(payload);

                    EventBusHandleResult<TResult> result = await TryHandleWithRetryPolicyAsync(
                        channel,
                        message,
                        queueBinding.Queue,
                        messageId,
                        args,
                        handler.HandleAsync);

                    await handler.HandleAfterAsync(_serviceProvider, result);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while handling a message");
                channel.BasicReject(args.DeliveryTag, false);
                throw;
            }

        };

    }

     private async Task<EventBusHandleResult<TResult>> TryHandleWithRetryPolicyAsync<TMessage, TResult>(
        IModel channel,
        TMessage message,
        string queue,
        string messageId,
        BasicDeliverEventArgs args,
        Func<IServiceProvider, TMessage, Task<TResult>> handle)
    {
        var handleResult = new EventBusHandleResult<TResult>(
            messageId,
            args.Exchange,
            queue,
            typeof(TMessage).AssemblyQualifiedName,
            _serializer.Serialize(message));

        handleResult.StartProcessing();

        var currentRetry = 0;
        var messageName = message?.GetType().Name.ToRabbitSnakeCase();
        var errors = new List<string>();
        var isSuccess = false;

        await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}'.";
                _logger.LogDebug("Handling a message: '{MessageName}' [id: '{MessageId}']'. {RetryMessage}", messageName, messageId, retryMessage);

                var result = await handle(_serviceProvider, message);
                handleResult.SetDetails(result);

                channel.BasicAck(args.DeliveryTag, false);
                _logger.LogDebug("Handled a message: '{MessageName}' [id: '{MessageId}']'. {RetryMessage}", messageName, messageId, retryMessage);

                isSuccess = true;
            }
            catch (Exception ex)
            {
                errors.Add(currentRetry != 0 ? $"Retry: {currentRetry}; Exception: {ex}" : $"Exception: {ex}");

                currentRetry++;

                _logger.LogError(ex, "Error while handling a message: '{MessageName}' [id: '{MessageId}']", messageName, messageId);

                if (currentRetry > 1)
                {
                    _logger.LogError(ex, "Unable to handle a message: '{MessageName}' [id: '{MessageId}'] retry {Retry}/{Retries}...", messageName, messageId, currentRetry - 1, _options.Retries);
                }

                if (currentRetry - 1 < 3)
                {
                    throw;
                }

                _logger.LogError(ex, "Handling a message: '{MessageName}' [id: '{MessageId}'] failed", messageName, messageId);
                channel.BasicNack(args.DeliveryTag, false, false);
            }
        });

        var handledRetriesCount = currentRetry == 0 ? 0 : currentRetry - 1;
        handleResult.StopProcessing(handledRetriesCount, isSuccess, errors.ToArray());

        return handleResult;
    }
}