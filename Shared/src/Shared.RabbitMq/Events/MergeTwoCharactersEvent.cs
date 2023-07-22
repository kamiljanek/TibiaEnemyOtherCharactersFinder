namespace Shared.RabbitMQ.Events;

/// <summary>
/// Event to merge correlations of old and new character
/// </summary>
/// <param name="OldCharacterId"></param>
/// <param name="NewCharacterId"></param>
public record MergeTwoCharactersEvent(int OldCharacterId, int NewCharacterId);
