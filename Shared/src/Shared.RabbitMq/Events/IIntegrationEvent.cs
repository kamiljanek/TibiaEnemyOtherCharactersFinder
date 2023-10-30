namespace Shared.RabbitMQ.Events;

public interface IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
}