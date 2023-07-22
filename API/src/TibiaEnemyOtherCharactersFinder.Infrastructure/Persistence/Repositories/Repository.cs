using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

// ReSharper disable RedundantAnonymousTypePropertyName

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence.Repositories;

public class Repository : IRepository
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public Repository(ITibiaCharacterFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
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
        await _dbContext.SaveChangesAsync();
    }

    public async Task UpdateWorldAsync(World newWorld)
    {   
        var currentWorld = await _dbContext.Set<World>().FirstOrDefaultAsync(e => e.WorldId == newWorld.WorldId);
        _dbContext.Entry(currentWorld).CurrentValues.SetValues(newWorld);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task SetCharacterFoundInScan(IReadOnlyList<string> charactersNames, bool foundInScan)
    {
        await _dbContext.Characters
           .Where(ch => charactersNames.Contains(ch.Name))
           .ExecuteUpdateAsync(update => update
               .SetProperty(c => c.FoundInScan, c => foundInScan));
    }

    public async Task ResetCharacterFoundInScan()
    {
        await _dbContext.Characters
           .Where(ch => ch.FoundInScan)
           .ExecuteUpdateAsync(update => update
               .SetProperty(c => c.FoundInScan, c => false));
        // UNDONE: sprawdzić czy ta metoda jest dalej potrzebna
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

    public async Task ReplaceCharacterIdInCharacterCorrelations(int oldCharacterId, int newCharacterId)
    {
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LoginCharacterId == oldCharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LoginCharacterId, newCharacterId));

        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LogoutCharacterId == oldCharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LogoutCharacterId, newCharacterId));
    }

    /// <param name="rawSql">Sql command to execute</param>
    /// <param name="timeOut">Optional value in seconds</param>
    public async Task ExecuteRawSqlAsync(string rawSql, int? timeOut)
    {
        if (timeOut is not null)
        {
            _dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds((double)timeOut));
        }

        await _dbContext.Database.ExecuteSqlRawAsync(rawSql);
    }

    public async Task DeleteIrrelevantCharacterCorrelations(int numberOfDays, int matchingNumber)
    {
        var thresholdDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-numberOfDays));

        _dbContext.Database.SetCommandTimeout(600);

        await _dbContext.CharacterCorrelations
            .Where(c => c.NumberOfMatches < matchingNumber && c.LastMatchDate < thresholdDate)
            .ExecuteDeleteAsync();
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
                .SelectMany(wc => wc.LoginCharacterCorrelations.Select(cc => new {LoginCharacterId = cc.LoginCharacterId, LogoutCharacterId = cc.LogoutCharacterId}));
        
        var existingCharacterCorrelationsPart2 =
            _dbContext.Characters
                .Where(c => logoutCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginCharacterCorrelations.Select(cc => new {LoginCharacterId = cc.LogoutCharacterId, LogoutCharacterId = cc.LoginCharacterId}));
  
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

        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCharacterCorrelationIfCorrelationExistInScan()
    {
        var charactersToRemove = _dbContext.Characters.Where(c => c.FoundInScan).Select(c => c.CharacterId);

        await _dbContext.CharacterCorrelations
        .Where(cc => charactersToRemove.Contains(cc.LoginCharacterId) && charactersToRemove.Contains(cc.LogoutCharacterId)).ExecuteDeleteAsync();
    }

    public async Task ClearChangeTracker()
    {
        await Task.Run(() => _dbContext.ChangeTracker.Clear());
    }

    public async Task<Character> GetFirstCharacterByVerifiedDate()
    {
        return await _dbContext.Characters
            .Where(c => c.TradedDate == null || c.TradedDate < DateOnly.FromDateTime(DateTime.Now.AddDays(-30)))
            .OrderByDescending(c => c.VerifiedDate == null)
            .ThenBy(c => c.VerifiedDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> SqlQueryRaw<T>(string query, params object[] parameters) where T : class
    {
        return await Task.Run(() => _dbContext.Database.SqlQueryRaw<T>(query, parameters).AsEnumerable());
    }

    public async Task DeleteAsync<T>(T entity) where T : class, IEntity
    {
        await Task.Run(() => _dbContext.Set<T>().Remove(entity));
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteCharacterCorrelationsByCharacterId(int characterId)
    {
        await _dbContext.CharacterCorrelations
            .Where(c => c.LoginCharacterId == characterId || c.LogoutCharacterId == characterId)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteCharacterCorrelationsByIds(IReadOnlyList<int> characterCorrelationsIds)
    {
        await _dbContext.CharacterCorrelations
            .Where(c => characterCorrelationsIds.Contains(c.CorrelationId))
            .ExecuteDeleteAsync();
    }

    public async Task UpdateVerifiedDate(int characterId)
    {
        await _dbContext.Characters
            .Where(cc => cc.CharacterId == characterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.VerifiedDate, DateOnly.FromDateTime(DateTime.Now)));
    }

    public async Task ReplaceCharacterIdInCharacterCorrelations(Character oldCharacter, Character newCharacter)
    {
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LoginCharacterId == oldCharacter.CharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LoginCharacterId, newCharacter.CharacterId));
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LogoutCharacterId == oldCharacter.CharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LogoutCharacterId, newCharacter.CharacterId));
    }

    public async Task<Character> GetCharacterByName(string characterName)
    {
        return await _dbContext.Characters.Where(c => c.Name == characterName.ToLower()).FirstOrDefaultAsync();
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
