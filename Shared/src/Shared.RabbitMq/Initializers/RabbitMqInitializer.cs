using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shared.RabbitMQ.Configuration;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;

namespace Shared.RabbitMQ.Initializers;

public class RabbitMqInitializer : IRabbitMqInitializer
{
    private readonly IConnection _connection;
    private readonly RabbitMqSection _options;
    private readonly ILogger<RabbitMqInitializer> _logger;
    private readonly EventBusSubscriberBuilder _eventBusSubscriberBuilder;

    public RabbitMqInitializer(RabbitMqConnection connection,
        IOptions<RabbitMqSection> options,
        ILogger<RabbitMqInitializer> logger,
        EventBusSubscriberBuilder eventBusSubscriberBuilder)
    {
        _connection = connection.Connection;
        _options = options.Value;
        _logger = logger;
        _eventBusSubscriberBuilder = eventBusSubscriberBuilder;
    }
    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        using (IModel channel = _connection.CreateModel())
        {
            var exchangeOptions = _options.Exchange;
            var deadLetterOptions = _options.DeadLetter;

            _logger.LogDebug("Declaring exchange: '{ExchangeName}' type: '{ExchangeType}'", exchangeOptions.Name, nameof(ExchangeType.Direct));
            channel.ExchangeDeclare(exchangeOptions.Name, ExchangeType.Direct, exchangeOptions.Durable, exchangeOptions.AutoDelete);

            var deadLetterExchangeName = $"{deadLetterOptions.Prefix}{exchangeOptions.Name}";
            _logger.LogDebug("Declaring exchange: '{DeadLetterExchange}' type: '{ExchangeType}'", deadLetterExchangeName, nameof(ExchangeType.Direct));
            channel.ExchangeDeclare(deadLetterExchangeName, ExchangeType.Direct, deadLetterOptions.Durable, deadLetterOptions.AutoDelete);

            var queueOptions = _options.Queue;

            IEnumerable<QueueBinding> bindings = _eventBusSubscriberBuilder.GetBindings();

            foreach (var queueBinding in bindings)
            {
                var deadLetterQueueName = $"{deadLetterOptions.Prefix}{queueBinding.Queue}";

                Dictionary<string, object> queueArguments = new()
                {
                    { "x-dead-letter-exchange", deadLetterExchangeName },
                    { "x-dead-letter-routing-key", deadLetterQueueName }
                };

                _logger.LogDebug(
                    "Declaring queue: '{Queue}' with routing: '{RoutingKey}'", queueBinding.Queue,
                    queueBinding.RoutingKey);

                channel.QueueDeclare(queueBinding.Queue, queueOptions.Durable, queueOptions.Exclusive,
                    queueOptions.AutoDelete, queueArguments);
                channel.QueueBind(queueBinding.Queue, queueBinding.Exchange, queueBinding.RoutingKey);

                Dictionary<string, object> deadLetterArguments = new()
                {
                    { "x-dead-letter-exchange", queueBinding.Exchange },
                    { "x-dead-letter-routing-key", queueBinding.Queue }
                };

                _logger.LogDebug("Declaring queue: '{DeadLetterQueueName}' with routing: '{DeadLetterRoutingKey}'",
                    deadLetterQueueName, deadLetterQueueName);

                channel.QueueDeclare(deadLetterQueueName, deadLetterOptions.Durable, deadLetterOptions.Exclusive,
                    deadLetterOptions.AutoDelete, deadLetterArguments);
                channel.QueueBind(deadLetterQueueName, deadLetterExchangeName, deadLetterQueueName);
            }

            channel.Close();
        }
        return Task.CompletedTask;
    }
}