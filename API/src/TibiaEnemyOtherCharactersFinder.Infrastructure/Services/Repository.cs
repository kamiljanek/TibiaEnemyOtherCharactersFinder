using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public class Repository : IRepository
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public Repository(ITibiaCharacterFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<World>> GetAvailableWorldsAsync() =>
        Task.FromResult(_dbContext.Worlds.Where(w => w.IsAvailable).ToList());
    
    public Task<List<World>> GetWorldsAsNoTrackingAsync() =>
        Task.FromResult(_dbContext.Worlds.AsNoTracking().ToList());

    public Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId) =>
        Task.FromResult(_dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .ToList());

    public Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync() =>
       Task.FromResult(_dbContext.WorldScans
           .Where(scan => !scan.IsDeleted)
           .Select(scan => scan.WorldId)
           .Distinct()
           .ToList());
    
    public Task<List<short>> GetAvailableWorldIdsFromWorldScansAsync() =>
       Task.FromResult(_dbContext.WorldScans
           .Where(scan => !scan.IsDeleted)
           .Select(scan => scan.WorldId)
           .ToList());

    public async Task SoftDeleteWorldScanAsync(WorldScan worldScan)
    {
        worldScan.IsDeleted = true;
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddCharactersActionsAsync(List<CharacterAction> characterActions)
    {
        _dbContext.CharacterActions.AddRange(characterActions);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddAsync<T>(T entity) where T : class, IEntity
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IEntity
    {
        _dbContext.Set<T>().AddRange(entities);
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateWorldAsync(World newWorld)
    {   
        var currentWorld = await _dbContext.Set<World>().FirstOrDefaultAsync(e => e.WorldId == newWorld.WorldId);
        _dbContext.Entry(currentWorld).CurrentValues.SetValues(newWorld);
        await _dbContext.SaveChangesAsync();
    }
}