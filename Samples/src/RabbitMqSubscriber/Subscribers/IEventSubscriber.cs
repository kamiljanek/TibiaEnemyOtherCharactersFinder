using RabbitMQ.Client.Events;

namespace RabbitMqSubscriber.Subscribers;

public interface IEventSubscriber
{
    string GetQueueName();
    Task OnReceived(BasicDeliverEventArgs ea, CancellationToken cancellationToken = default);
}