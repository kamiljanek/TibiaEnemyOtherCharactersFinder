using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public interface IRepository
{
    public Task<List<World>> GetAvailableWorldsAsync();

    public Task<List<World>> GetWorldsAsNoTrackingAsync();

    public Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId);

    public Task<HashSet<short>> GetDistinctWorldIdsFromWorldScansAsync();

    public Task SoftDeleteWorldScanAsync(WorldScan worldScan);

    public Task AddCharactersActionsAsync(List<CharacterAction> characterActions);
}