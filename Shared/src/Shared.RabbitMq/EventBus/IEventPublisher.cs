namespace Shared.RabbitMQ.EventBus;

public interface IEventPublisher
{
    Task PublishAsync(string messageId, object message);
}