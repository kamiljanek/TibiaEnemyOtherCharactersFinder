using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace TibiaEnemyOtherCharactersFinder.Application.Persistence;

public interface IRepository
{
    // UNDONE: pododawać wszędzie CancellationToken
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    Task<List<World>> GetAvailableWorldsAsync();
    Task<List<World>> GetWorldsAsNoTrackingAsync();
    Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId);
    Task<List<short>> GetDistinctWorldIdsFromWorldScansAsync();
    Task<int> NumberOfAvailableWorldScansAsync();
    Task SoftDeleteWorldScanAsync(int worldScan);
    Task AddAsync<T>(T entity) where T : class, IEntity;
    Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class, IEntity;
    Task UpdateWorldAsync(World entity);
    Task UpdateCharacterCorrelations();
    Task ExecuteRawSqlAsync(string rawSql, int? timeOut = null);
    Task CreateCharacterCorrelationsIfNotExist();
    Task SetCharacterFoundInScan(IReadOnlyList<string> charactersNames, bool foundInScan);
    Task DeleteIrrelevantCharacterCorrelations(int numberOfDays, int matchingNumber);
    Task DeleteCharacterCorrelationIfCorrelationExistInScan();
    Task ClearChangeTracker();
    Task<Character> GetFirstCharacterByVerifiedDate();
    Task<IEnumerable<T>> SqlQueryRaw<T>(string query, params object[] parameters) where T : class;
    Task DeleteAsync<T>(T entity) where T : class, IEntity;
    Task DeleteCharacterCorrelationsByCharacterId(int characterId);
    Task UpdateVerifiedDate(int characterId);
    Task ReplaceCharacterIdInCharacterCorrelations(Character oldCharacter, Character newCharacter);
    Task<Character> GetCharacterByName(string characterName);
    Task DeleteCharacterCorrelationsByIds(IReadOnlyList<int> characterCorrelationsIds);

}