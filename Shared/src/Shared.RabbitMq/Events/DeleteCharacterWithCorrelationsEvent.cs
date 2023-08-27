namespace Shared.RabbitMQ.Events;

/// <summary>
/// Event to delete character with all correlations
/// </summary>
/// <param name="CharacterId"></param>
public record DeleteCharacterWithCorrelationsEvent(int CharacterId) : IntegrationEvent;
