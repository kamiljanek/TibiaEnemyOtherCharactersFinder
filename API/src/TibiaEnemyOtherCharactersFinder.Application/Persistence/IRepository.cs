using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace TibiaEnemyOtherCharactersFinder.Application.Persistence;

public interface IRepository
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
    Task<List<World>> GetAvailableWorldsAsync(CancellationToken cancellationToken = default);
    Task<List<World>> GetWorldsAsNoTrackingAsync(CancellationToken cancellationToken = default);
    Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId, CancellationToken cancellationToken = default);
    Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync(CancellationToken cancellationToken = default);
    int NumberOfAvailableWorldScans();
    Task SoftDeleteWorldScanAsync(int worldScan, CancellationToken cancellationToken = default);
    Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
    Task AddRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default) where T : class, IEntity;
    Task UpdateWorldAsync(World entity, CancellationToken cancellationToken = default);
    Task UpdateCharacterCorrelationsAsync(CancellationToken cancellationToken = default);
    Task ExecuteRawSqlAsync(string rawSql, int? timeOut = null, CancellationToken cancellationToken = default);
    Task CreateCharacterCorrelationsIfNotExistAsync(CancellationToken cancellationToken = default);
    Task SetCharacterFoundInScanAsync(IReadOnlyList<string> charactersNames, bool foundInScan, CancellationToken cancellationToken = default);
    Task DeleteIrrelevantCharacterCorrelationsAsync(int numberOfDays, int matchingNumber, CancellationToken cancellationToken = default);
    Task DeleteCharacterCorrelationIfCorrelationExistInScanAsync(CancellationToken cancellationToken = default);
    void ClearChangeTracker();
    Task<Character> GetFirstCharacterByVerifiedDateAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> SqlQueryRaw<T>(string query, params object[] parameters) where T : class;
    Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class, IEntity;
    Task DeleteCharacterCorrelationsByCharacterIdAsync(int characterId, CancellationToken cancellationToken = default);
    Task ReplaceCharacterIdInCorrelationsAsync(Character oldCharacter, Character newCharacter, CancellationToken cancellationToken = default);
    Task<Character> GetCharacterByNameAsync(string characterName, CancellationToken cancellationToken = default);
    Task<Character> GetCharacterByIdAsync(int characterId, CancellationToken cancellationToken = default);
    Task DeleteCharacterCorrelationsByIdsAsync(IReadOnlyList<int> characterCorrelationsIds, CancellationToken cancellationToken = default);

}