using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Shared.RabbitMQ.Configuration;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.EventBus;

namespace Shared.RabbitMQ.Events;

public class RabbitMqBusPublisher : IEventBusPublisher
{
    private readonly IRabbitMqConventionProvider _conventionProvider;
    private readonly ILogger<RabbitMqBusPublisher> _logger;
    private readonly PublisherConnection _connection;
    private readonly MessageSerializer _serializer;
    private readonly RabbitMqSection _options;

    public RabbitMqBusPublisher(IRabbitMqConventionProvider conventionProvider,
        ILogger<RabbitMqBusPublisher> logger,
        IOptions<RabbitMqSection> options,
        PublisherConnection connection,
        MessageSerializer serializer)
    {
        _conventionProvider = conventionProvider;
        _logger = logger;
        _connection = connection;
        _serializer = serializer;
        _options = options.Value;
    }

    public Task PublishAsync(string messageId, object message)
    {
        if (!_connection.IsConnected)
        {
            _logger.LogWarning("Could not publish message: {MessageId}. RabbitMq connection is refused", messageId);
            return Task.CompletedTask;
        }

        var policy = GetRetryPolicy(
            (ex, time) =>
            {
                _logger.LogWarning(ex,
                    "Could not publish message: {MessageName} after {TotalSeconds:n1}s {ExceptionMessage}",
                    messageId, time.TotalSeconds, ex.Message);
            });

        var queueBinding = _conventionProvider.GetForType(message.GetType());

        using IModel channel = _connection.Connection.CreateModel();

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = messageId;
        properties.Timestamp = new AmqpTimestamp(DateTimeOffset.Now.ToUnixTimeSeconds());

        var serializedMessage = _serializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(serializedMessage);

        policy.Execute(() => channel.BasicPublish(queueBinding.Exchange, queueBinding.RoutingKey, mandatory: true, properties, body));

        _logger.LogInformation("Event published. Event: {Event}, MessageId: {MessageId}", message.GetType().Name, messageId);

        return Task.CompletedTask;
    }

    private RetryPolicy GetRetryPolicy(Action<Exception, TimeSpan> onRetryAction) =>
        Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(_options.Retries,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(_options.RetryInterval, retryAttempt)),
                onRetryAction);
}