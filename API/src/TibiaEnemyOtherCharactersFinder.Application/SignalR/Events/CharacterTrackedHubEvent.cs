namespace TibiaEnemyOtherCharactersFinder.Application.SignalR.Events;

public record CharacterTrackedHubEvent(string Name, bool IsOnline)
{
    public Guid HubEventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}