namespace Shared.RabbitMQ.Events;

public interface IIntegrationEventPublisher
{
    Task PublishEventAsync<T>(T @event, CancellationToken cancellationToken = default)
        where T : IIntegrationEvent;
}