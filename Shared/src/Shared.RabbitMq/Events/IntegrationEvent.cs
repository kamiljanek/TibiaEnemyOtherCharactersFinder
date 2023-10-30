namespace Shared.RabbitMQ.Events;

public abstract record IntegrationEvent : IIntegrationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.Now;
}