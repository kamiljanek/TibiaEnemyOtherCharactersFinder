using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Application.Services;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Hubs;

public class CharactersTrackHub : Hub
{
    private readonly ILogger<CharactersTrackHub> _logger;
    private readonly ITrackCharacterService _trackCharacterService;

    public CharactersTrackHub(ILogger<CharactersTrackHub> logger, ITrackCharacterService trackCharacterService)
    {
        _logger = logger;
        _trackCharacterService = trackCharacterService;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation(
            "Client connected. ConnectionId: {ConnectionId}. UserId: {ContextUserId}",
            Context.ConnectionId, Context.UserIdentifier);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _trackCharacterService.RemoveTracksByConnectionId(Context.ConnectionId);

        _logger.LogInformation(
            "Client disconnected. ConnectionId: {ConnectionId}. UserId: {ContextUserId}, Exception: {Exception}",
            Context.ConnectionId, Context.UserIdentifier, exception?.ToString());
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// External method for external services e.g. Web Application
    /// </summary>
    /// <param name="groupName">Name of group to track specific character</param>
    public async Task JoinGroup(string groupName)
    {
        groupName = groupName.ToLower();

        await _trackCharacterService.CreateTrack(groupName, Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    /// <summary>
    /// External method for external services e.g. Web Application
    /// </summary>
    /// <param name="groupName">Name of group to track specific character</param>
    public async Task LeaveGroup(string groupName)
    {
        groupName = groupName.ToLower();

        await _trackCharacterService.RemoveTrack(groupName, Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}