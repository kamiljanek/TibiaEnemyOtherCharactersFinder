using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Application.Exceptions;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public class TrackCharacterService : ITrackCharacterService
{
    private readonly ITibiaDataClient _tibiaDataClient;
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public TrackCharacterService(ITibiaDataClient tibiaDataClient, ITibiaCharacterFinderDbContext dbContext)
    {
        _tibiaDataClient = tibiaDataClient;
        _dbContext = dbContext;
    }

    public async Task CreateTrack(string characterName, string connectionId)
    {
        var fetchedCharacter = await _tibiaDataClient.FetchCharacter(characterName);

        if (fetchedCharacter is null)
        {
            throw new TibiaDataApiConnectionException();
        }
        if (string.IsNullOrWhiteSpace(fetchedCharacter.Name))
        {
            throw new NotFoundException(nameof(Character), characterName);
        }

        var entity = new TrackedCharacter(characterName, fetchedCharacter.World, connectionId);
        _dbContext.TrackedCharacters.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveTrack(string characterName, string connectionId)
    {
        await _dbContext.TrackedCharacters.Where(tc => tc.Name == characterName && tc.ConnectionId == connectionId)
            .ExecuteDeleteAsync();
    }

    public async Task RemoveTracksByConnectionId(string connectionId)
    {
        await _dbContext.TrackedCharacters.Where(tc => tc.ConnectionId == connectionId)
            .ExecuteDeleteAsync();
    }
}