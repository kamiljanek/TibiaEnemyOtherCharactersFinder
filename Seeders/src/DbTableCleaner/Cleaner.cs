using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace DbCleaner;

public class Cleaner : ICleaner
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public Cleaner(ITibiaCharacterFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ClearUnnecessaryWorldScans()
    {
        await _dbContext.ExecuteRawSqlAsync(GenerateQueries.ClearDeletedWorldScans, timeOut: 60);

        await _dbContext.WorldScans
            .Where(ws => !ws.World.IsAvailable)
            .ExecuteDeleteAsync();
    }

    public async Task TruncateCharacterActions()
    {
        await _dbContext.ExecuteRawSqlAsync("TRUNCATE TABLE character_actions RESTART IDENTITY;", timeOut: 60);
    }

    public async Task DeleteIrrelevantCharacterCorrelations()
    {
        var thresholdDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-30));

        _dbContext.Database.SetCommandTimeout(600);

        await _dbContext.CharacterCorrelations
            .Where(c => c.NumberOfMatches < 3 && c.LastMatchDate < thresholdDate)
            .ExecuteDeleteAsync();
    }

    public async Task VacuumCharacterActions()
    {
        await _dbContext.ExecuteRawSqlAsync("VACUUM FULL character_actions", timeOut: 60);
    }

    public async Task VacuumWorldScans()
    {
        await _dbContext.ExecuteRawSqlAsync("VACUUM FULL world_scans", timeOut: 60);
    }

    public async Task VacuumCharacters()
    {
        await _dbContext.ExecuteRawSqlAsync("VACUUM FULL characters", timeOut: 300);
    }

    public async Task VacuumWorlds()
    {
        await _dbContext.ExecuteRawSqlAsync("VACUUM FULL worlds", timeOut: 60);
    }

    public async Task VacuumCharacterCorrelations()
    {
        await _dbContext.ExecuteRawSqlAsync("VACUUM FULL character_correlations", timeOut: 600);
    }
}