namespace Shared.RabbitMQ.EventBus;

public interface IEventBusPublisher
{
    Task PublishAsync(string messageId, object message);
}