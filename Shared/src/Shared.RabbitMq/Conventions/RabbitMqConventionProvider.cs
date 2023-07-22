using Microsoft.Extensions.Options;
using Shared.RabbitMQ.Configuration;

namespace Shared.RabbitMQ.Conventions;

public class RabbitMqConventionProvider : IRabbitMqConventionProvider
{
    private readonly RabbitMqSection _options;
    private readonly Dictionary<Type, QueueBinding> _typeBindings = new();

    public RabbitMqConventionProvider(IOptions<RabbitMqSection> options)
    {
        _options = options.Value;
    }

    public QueueBinding GetForType<T>() => CreateBinding(typeof(T));

    public QueueBinding GetForType(Type messageType) => CreateBinding(messageType);

    private QueueBinding CreateBinding(Type messageType)
    {
        if (_typeBindings.TryGetValue(messageType, out QueueBinding existingBinding))
        {
            return existingBinding;
        }

        var routingKey = messageType.Name;
        var exchange = _options.Exchange.Name;
        var queue = $"{exchange}.{routingKey}";

        var queueBinding = new QueueBinding(routingKey.ToRabbitSnakeCase(), exchange.ToRabbitSnakeCase(),
            queue.ToRabbitSnakeCase());
        _typeBindings.TryAdd(messageType, queueBinding);

        return queueBinding;
    }
}