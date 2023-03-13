using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

public class Repository : IRepository
{
    private readonly TibiaCharacterFinderDbContext _dbContext;

    public Repository(TibiaCharacterFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<World>> GetAvailableWorldsAsync()
    {
        return Task.FromResult(_dbContext.Worlds.Where(w => w.IsAvailable).ToList());
    }

    public Task<List<World>> GetWorldsAsNoTrackingAsync()
    {
        return Task.FromResult(_dbContext.Worlds.AsNoTracking().ToList());
    }

    public Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId)
    {
        return Task.FromResult(_dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .AsNoTracking()
            .ToList());
    }

    public Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync()
    {
        return Task.FromResult(_dbContext.WorldScans
            .Where(scan => !scan.IsDeleted)
            .Select(scan => scan.WorldId)
            .Distinct()
            .OrderBy(id => id)
            .ToList());
    }

    public Task<int> NumberOfAvailableWorldScansAsync()
    {
        return Task.FromResult(_dbContext.WorldScans
            .Count(scan => !scan.IsDeleted));
    }

    public async Task SoftDeleteWorldScanAsync(int worldScanId)
    {
        await _dbContext.WorldScans
            .Where(ws => ws.WorldScanId == worldScanId)
            .ExecuteUpdateAsync(update => update.SetProperty(ws => ws.IsDeleted, true));
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
        
        // UNDONE: do zmiany
    }
    
    public async Task SetCharacterFoundInScan(IReadOnlyList<string> charactersNames, bool foundInScan)
    {
        await _dbContext.Characters
           .Where(ch => charactersNames.Contains(ch.Name))
           .ExecuteUpdateAsync(update => update
               .SetProperty(c => c.FoundInScan, c => foundInScan));
    }

    public async Task UpdateCharacterCorrelations()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).LogoutOrLoginDate;
        var loginCharactersIds = CharactersIds(true);
        var logoutCharactersIds = CharactersIds(false);

        var characterCorrelationsIdsPart1 =  _dbContext.CharacterCorrelations
            .Where(c => loginCharactersIds.Contains(c.LoginCharacterId) && logoutCharactersIds.Contains(c.LogoutCharacterId))
            .Select(cc => cc.CorrelationId);

        var characterCorrelationsIdsPart2 =  _dbContext.CharacterCorrelations
            .Where(c => logoutCharactersIds.Contains(c.LoginCharacterId) && loginCharactersIds.Contains(c.LogoutCharacterId))
            .Select(cc => cc.CorrelationId);


        await _dbContext.CharacterCorrelations
           .Where(cc => characterCorrelationsIdsPart1.Concat(characterCorrelationsIdsPart2).Contains(cc.CorrelationId))
           .ExecuteUpdateAsync(update => update
               .SetProperty(c => c.NumberOfMatches, c => c.NumberOfMatches + 1)
               .SetProperty(c => c.LastMatchDate, lastMatchDate));
    }

    /// <param name="rawSql">Sql command to execute</param>
    /// <param name="timeOut">Value in seconds</param>
    public async Task ExecuteRawSqlAsync(string rawSql, int timeOut)
    {   
        _dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds(timeOut));
        await _dbContext.Database.ExecuteSqlRawAsync(rawSql);
    }

    public async Task CreateCharacterCorrelationsIfNotExist()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).LogoutOrLoginDate;
        var loginCharactersIds = CharactersIds(true);
        var logoutCharactersIds = CharactersIds(false);
        
        var correlationsDataToCreate = loginCharactersIds
            .SelectMany(login => logoutCharactersIds,
                (login, logout) => new { LoginCharacterId = login, LogoutCharacterId = logout });
        
        var existingCharacterCorrelationsPart1 =
            _dbContext.Characters
                .Where(c => loginCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginCharacterCorrelations.Select(wc => new {LoginCharacterId = wc.LoginCharacterId, LogoutCharacterId = wc.LogoutCharacterId}));
        
        var existingCharacterCorrelationsPart2 =
            _dbContext.Characters
                .Where(c => logoutCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginCharacterCorrelations.Select(wc => new {LoginCharacterId = wc.LogoutCharacterId, LogoutCharacterId = wc.LoginCharacterId}));
  
        var correlationsDataToInsert = correlationsDataToCreate.Except(existingCharacterCorrelationsPart1).Except(existingCharacterCorrelationsPart2);
     
        var newCorrelations = correlationsDataToInsert
            .Select(cc => new CharacterCorrelation
            {
                LoginCharacterId = cc.LoginCharacterId,
                LogoutCharacterId = cc.LogoutCharacterId,
                NumberOfMatches = 1,
                CreateDate = lastMatchDate,
                LastMatchDate = lastMatchDate
            }).ToList();

        _dbContext.CharacterCorrelations.AddRange(newCorrelations);
        await _dbContext.BulkSaveChangesAsync(x => x.BatchSize = 30);
    }
    
    private IQueryable<int> CharactersIds(bool isOnline)
    {
        return _dbContext.Characters
            .Where(c =>
                _dbContext.CharacterActions
                    .Where(ca => ca.IsOnline == isOnline)
                    .Select(ca => ca.CharacterName)
                    .Distinct().Contains(c.Name))
            .Select(ca => ca.CharacterId);
    }
}
