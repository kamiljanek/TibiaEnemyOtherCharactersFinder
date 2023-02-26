using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using Z.EntityFramework.Plus;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public class Repository : IRepository
{
    private readonly TibiaCharacterFinderDbContext _dbContext;

    public Repository(TibiaCharacterFinderDbContext dbContext)
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
            .AsTracking()
            .ToList());

    public Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync() =>
       Task.FromResult(_dbContext.WorldScans
           .Where(scan => !scan.IsDeleted)
           .Select(scan => scan.WorldId)
           .Distinct()
           .OrderBy(id => id)
           .ToList());
    
    public Task<int> NumberOfAvailableWorldScansAsync() =>
       Task.FromResult(_dbContext.WorldScans
           .Count(scan => !scan.IsDeleted));

    public async Task SoftDeleteWorldScanAsync(int worldScanId)
    {
        var worldScan =  await _dbContext.WorldScans.FirstOrDefaultAsync(ws => ws.WorldScanId == worldScanId);
        worldScan.IsDeleted = true;
        await _dbContext.BulkSaveChangesAsync(x => x.BatchSize = 30);
    }

    public async Task AddAsync<T>(T entity) where T : class, IEntity
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IEntity
    {
        _dbContext.Set<T>().AddRange(entities);
        
        await _dbContext.BulkSaveChangesAsync(x => x.BatchSize = 30);
    }

    public async Task UpdateWorldAsync(World newWorld)
    {   
        var currentWorld = await _dbContext.Set<World>().FirstOrDefaultAsync(e => e.WorldId == newWorld.WorldId);
        _dbContext.Entry(currentWorld).CurrentValues.SetValues(newWorld);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveCharacterCorrelations(bool isOnline)
    {
        var charactersIds = _dbContext.Characters
            .Where(c =>
                _dbContext.CharacterActions
                    .Where(ca => ca.IsOnline == isOnline)
                    .Select(ca => ca.CharacterName)
                    .Distinct().Contains(c.Name))
            .Select(ca => ca.CharacterId)
            .ToArray();
        
        await _dbContext.CharacterCorrelations
            .Where(c => charactersIds.Contains(c.LoginCharacterId) && charactersIds.Contains(c.LogoutCharacterId))
            .DeleteAsync(x => x.BatchSize = 30);
    }
}