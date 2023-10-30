namespace Shared.RabbitMQ.Conventions;

public interface IRabbitMqConventionProvider
{
    QueueBinding GetForType<T>();
    QueueBinding GetForType(Type messageType);
}