namespace Shared.RabbitMQ.Events;

public interface IIntegrationEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
    // UNDONE: do przemyślenia co tu ma być
}