using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

// ReSharper disable RedundantAnonymousTypePropertyName

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence.Repositories;

public class Repository : IRepository
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly ILogger<Repository> _logger;

    public Repository(ITibiaCharacterFinderDbContext dbContext, ILogger<Repository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<bool> ExecuteInTransactionAsync(Func<Task> action)
    {
        for (int retryCount = 0; retryCount < 3; retryCount++)
        {
            await using var transaction =
                await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable);

            try
            {
                await action.Invoke();
                await transaction.CommitAsync();
                _logger.LogInformation("Transaction '{action}' commited properly", action.Target?.GetType().ReflectedType?.FullName);
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError("Transaction failed: {ErrorMessage}", ex.Message);
            }
        }

        return false;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return default;
    }

    public async Task<List<World>> GetAvailableWorldsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Worlds.Where(w => w.IsAvailable).ToListAsync(cancellationToken);
    }

    public async Task<List<World>> GetWorldsAsNoTrackingAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Worlds.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.WorldScans
            .Where(scan => !scan.IsDeleted)
            .Select(scan => scan.WorldId)
            .Distinct()
            .OrderBy(id => id)
            .ToListAsync(cancellationToken);
    }

    public int NumberOfAvailableWorldScans()
    {
        return _dbContext.WorldScans.Count(scan => !scan.IsDeleted);
    }

    public async Task SoftDeleteWorldScanAsync(int worldScanId, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorldScans
            .Where(ws => ws.WorldScanId == worldScanId)
            .ExecuteUpdateAsync(update => update.SetProperty(ws => ws.IsDeleted, true), cancellationToken);
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity
    {
        _dbContext.Set<T>().Add(entity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity
    {
        _dbContext.Set<T>().AddRange(entities);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateWorldAsync(World newWorld, CancellationToken cancellationToken = default)
    {   
        var currentWorld = await _dbContext.Set<World>().FirstOrDefaultAsync(e => e.WorldId == newWorld.WorldId, cancellationToken);
        _dbContext.Entry(currentWorld).CurrentValues.SetValues(newWorld);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCharacterNameAsync(string oldName, string newName, CancellationToken cancellationToken = default)
    {
        await _dbContext.Characters
            .Where(c => c.Name == oldName.ToLower())
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.Name, newName.ToLower()), cancellationToken);
    }

    public async Task UpdateCharacterVerifiedDate(int characterId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Characters
            .Where(c => c.CharacterId == characterId)
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.VerifiedDate, DateOnly.FromDateTime(DateTime.Now)), cancellationToken);
    }

    public async Task SetCharacterFoundInScanAsync(IReadOnlyList<string> charactersNames, bool foundInScan, CancellationToken cancellationToken = default)
    {
        await _dbContext.Characters
           .Where(ch => charactersNames.Contains(ch.Name))
           .ExecuteUpdateAsync(update => update.SetProperty(c => c.FoundInScan, c => foundInScan), cancellationToken);
    }

    public async Task UpdateCharacterCorrelationsAsync(CancellationToken cancellationToken = default)
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync(cancellationToken)).LogoutOrLoginDate;
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
               .SetProperty(c => c.LastMatchDate, lastMatchDate), cancellationToken);
    }

    /// <param name="rawSql">Sql command to execute</param>
    /// <param name="timeOut">Optional value in seconds</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    public async Task ExecuteRawSqlAsync(string rawSql, int? timeOut, CancellationToken cancellationToken = default)
    {
        if (timeOut is not null)
        {
            _dbContext.Database.SetCommandTimeout(TimeSpan.FromSeconds((double)timeOut));
        }

        await _dbContext.Database.ExecuteSqlRawAsync(rawSql, cancellationToken);
    }

    public async Task DeleteIrrelevantCharacterCorrelationsAsync(int numberOfDays, int matchingNumber, CancellationToken cancellationToken = default)
    {
        var thresholdDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-numberOfDays));

        _dbContext.Database.SetCommandTimeout(600);

        await _dbContext.CharacterCorrelations
            .Where(c => c.NumberOfMatches < matchingNumber && c.LastMatchDate < thresholdDate)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task CreateCharacterCorrelationsIfNotExistAsync(CancellationToken cancellationToken = default)
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync(cancellationToken)).LogoutOrLoginDate;
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

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteCharacterCorrelationIfCorrelationExistInScanAsync(
        CancellationToken cancellationToken = default)
    {
        var charactersToRemove = _dbContext.Characters.Where(c => c.FoundInScan).Select(c => c.CharacterId);

        await _dbContext.CharacterCorrelations
            .Where(cc => charactersToRemove.Contains(cc.LoginCharacterId) && charactersToRemove.Contains(cc.LogoutCharacterId))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task ClearChangeTracker()
    {
        await Task.Run(() => _dbContext.ChangeTracker.Clear());
    }

    public async Task<Character> GetFirstCharacterByVerifiedDateAsync(CancellationToken cancellationToken = default)
    {
        var thirtyDaysAgo = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

        return await _dbContext.Characters
            .Where(c => (!c.TradedDate.HasValue || c.TradedDate < thirtyDaysAgo) && (!c.VerifiedDate.HasValue || c.VerifiedDate < thirtyDaysAgo))
            .OrderByDescending(c => c.VerifiedDate == null)
            .ThenBy(c => c.VerifiedDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> SqlQueryRaw<T>(string query, params object[] parameters) where T : class
    {
        return await Task.Run(() => _dbContext.Database.SqlQueryRaw<T>(query, parameters).AsEnumerable());
    }

    public async Task DeleteCharacterByIdAsync(int characterId, CancellationToken cancellationToken = default)
    {
        await _dbContext.Characters
            .Where(c => c.CharacterId == characterId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteCharacterCorrelationsByCharacterIdAsync(int characterId, CancellationToken cancellationToken = default)
    {
        await _dbContext.CharacterCorrelations
            .Where(c => c.LoginCharacterId == characterId || c.LogoutCharacterId == characterId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteCharacterCorrelationsByIdsAsync(IReadOnlyList<int> characterCorrelationsIds, CancellationToken cancellationToken = default)
    {
        await _dbContext.CharacterCorrelations
            .Where(c => characterCorrelationsIds.Contains(c.CorrelationId))
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task ReplaceCharacterIdInCorrelationsAsync(Character oldCharacter, Character newCharacter, CancellationToken cancellationToken = default)
    {
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LoginCharacterId == oldCharacter.CharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LoginCharacterId, newCharacter.CharacterId), cancellationToken);
        await _dbContext.CharacterCorrelations
            .Where(cc => cc.LogoutCharacterId == oldCharacter.CharacterId)
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.LogoutCharacterId, newCharacter.CharacterId), cancellationToken);
    }

    public async Task<Character> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Characters.Where(c => c.Name == characterName.ToLower()).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Character> GetCharacterByIdAsync(int characterId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Characters.Where(c => c.CharacterId == characterId).FirstOrDefaultAsync(cancellationToken);
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
