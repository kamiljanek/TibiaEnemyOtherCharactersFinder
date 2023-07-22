using RabbitMQ.Client.Events;

namespace RabbitMqSubscriber.Events.Handlers;

public interface IEventSubscriber
{
    string GetQueueName();
    Task OnReceived(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default);
}