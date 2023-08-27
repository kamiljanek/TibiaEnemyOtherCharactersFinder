using Microsoft.Extensions.Logging;

namespace Shared.RabbitMQ.Events;

/// <summary>
/// Event to merge correlations of old and new character
/// </summary>
/// <param name="OldCharacterId"></param>
/// <param name="NewCharacterId"></param>
public record MergeTwoCharactersEvent(int OldCharacterId, int NewCharacterId) : IntegrationEvent;

// internal sealed class MergeTwoCharactersEventHandler : IIntegrationEventHandler<MergeTwoCharactersEvent>
// {
//     private readonly ILogger<MergeTwoCharactersEventHandler> _logger;
//
//     public MergeTwoCharactersEventHandler(
//         ILogger<MergeTwoCharactersEventHandler> logger)
//     {
//         _logger = logger;
//     }
//
//     public async Task HandleAsync(MergeTwoCharactersEvent @event, CancellationToken cancellationToken = default)
//     {
//         Console.WriteLine($"odebrane {@event.NewCharacterId}/{@event.OldCharacterId}");
//         _logger.LogInformation("Event subscribed. Payload: {Payload}", @event);
//         await Task.Delay(1000, cancellationToken);
//     }
// }

// UNDONE: do wywalenia zakomentowany kod
