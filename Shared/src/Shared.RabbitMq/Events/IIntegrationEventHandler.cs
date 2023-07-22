namespace Shared.RabbitMQ.Events;

public interface IIntegrationEventHandler<in TEvent> where TEvent : class, IIntegrationEvent
{
    Task HandleAsync(TEvent integrationEvent, CancellationToken cancellationToken = default);
}