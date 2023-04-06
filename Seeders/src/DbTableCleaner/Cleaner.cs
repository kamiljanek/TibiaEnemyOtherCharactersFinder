using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace DbCleaner;

public class Cleaner : ICleaner
{
    private readonly IRepository _repository;

    public Cleaner(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ClearTables()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlClearDeletedWorldScans);
        await _repository.ExecuteRawSqlAsync("TRUNCATE TABLE character_actions RESTART IDENTITY;", timeOut: 60);
        await _repository.DeleteIrrelevantCharacterCorrelations(numberOfDays: 14, matchingNumber: 5);
    }
    
    public async Task VacuumTables()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL character_actions", timeOut: 60);
        await _repository.ExecuteRawSqlAsync("VACUUM FULL world_scans", timeOut: 60);
        await _repository.ExecuteRawSqlAsync("VACUUM FULL characters", timeOut: 600);
        await _repository.ExecuteRawSqlAsync("VACUUM FULL worlds", timeOut: 60);
        await _repository.ExecuteRawSqlAsync("VACUUM FULL character_correlations", timeOut: 1200);
    }
}