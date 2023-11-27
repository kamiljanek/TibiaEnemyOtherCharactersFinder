using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace TibiaEnemyOtherCharactersFinder.Application.Persistence;

public interface IRepository
{
    Task<bool> ExecuteInTransactionAsync(Func<Task> action);
    Task<List<World>> GetAvailableWorldsAsync(CancellationToken cancellationToken = default);
    Task<List<World>> GetWorldsAsNoTrackingAsync(CancellationToken cancellationToken = default);
    Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId, CancellationToken cancellationToken = default);
    Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync(CancellationToken cancellationToken = default);
    int NumberOfAvailableWorldScans();
    Task SoftDeleteWorldScanAsync(int worldScan);
    Task AddAsync<T>(T entity) where T : class, IEntity;
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IEntity;
    Task UpdateWorldAsync(World entity);
    Task UpdateCharacterNameAsync(string oldName, string newName);
    Task UpdateCharacterVerifiedDate(int characterId);
    Task UpdateCharacterTradedDate(int characterId);
    Task UpdateCorrelationsIfExistAsync();
    Task ExecuteRawSqlAsync(string rawSql, int? timeOut = null, CancellationToken cancellationToken = default);
    Task CreateCorrelationsIfNotExistAsync();
    Task SetCharacterFoundInScanAsync(IReadOnlyList<string> charactersNames, bool foundInScan);
    Task DeleteIrrelevantCharacterCorrelationsAsync(int numberOfDays, int matchingNumber);
    Task RemoveImposibleCorrelationsAsync();
    Task ClearChangeTracker();
    Task<Character> GetFirstCharacterByVerifiedDateAsync();
    Task<IEnumerable<T>> SqlQueryRaw<T>(string query, params object[] parameters) where T : class;
    Task DeleteCharacterByIdAsync(int characterId);
    Task DeleteCorrelationsByCharacterIdAsync(int characterId);
    Task ReplaceCharacterIdInCorrelationsAsync(Character oldCharacter, Character newCharacter);
    Task<Character> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken = default);
    Task<Character> GetCharacterByIdAsync(int characterId, bool withTracking = false, CancellationToken cancellationToken = default);
    Task DeleteCorrelationsByIdsAsync(IReadOnlyList<long> characterCorrelationsIds);
    Task DeleteOldWorldScansAsync();
}