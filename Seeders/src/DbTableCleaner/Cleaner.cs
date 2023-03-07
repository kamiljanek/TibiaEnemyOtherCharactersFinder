using Dapper;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
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
        await _repository.ExecuteRawSqlAsync("TRUNCATE TABLE character_actions RESTART IDENTITY;");
    }
    
    public async Task VacuumTables()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL character_actions");
        await _repository.ExecuteRawSqlAsync("VACUUM FULL world_scans");
        await _repository.ExecuteRawSqlAsync("VACUUM FULL characters");
        await _repository.ExecuteRawSqlAsync("VACUUM FULL worlds");
        await _repository.ExecuteRawSqlAsync("VACUUM FULL character_correlations");
    }
}