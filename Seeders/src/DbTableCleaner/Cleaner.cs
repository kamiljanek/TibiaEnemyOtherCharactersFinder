using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace DbCleaner;

public class Cleaner : ICleaner
{
    private readonly IRepository _repository;

    public Cleaner(IRepository repository)
    {
        _repository = repository;
    }

    public async Task ClearDeletedWorldScans()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.NpgsqlClearDeletedWorldScans, timeOut: 60);
    }

    public async Task TruncateCharacterActions()
    {
        await _repository.ExecuteRawSqlAsync("TRUNCATE TABLE character_actions RESTART IDENTITY;", timeOut: 60);
    }

    public async Task DeleteIrrelevantCharacterCorrelations()
    {
        await _repository.DeleteIrrelevantCharacterCorrelations(numberOfDays: 30, matchingNumber: 3);
    }

    public async Task VacuumCharacterActions()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL character_actions", timeOut: 60);
    }

    public async Task VacuumWorldScans()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL world_scans", timeOut: 60);
    }

    public async Task VacuumCharacters()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL characters", timeOut: 300);
    }

    public async Task VacuumWorlds()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL worlds", timeOut: 60);
    }

    public async Task VacuumCharacterCorrelations()
    {
        await _repository.ExecuteRawSqlAsync("VACUUM FULL character_correlations", timeOut: 600);
    }
}