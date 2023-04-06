using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public class DataBaseInitializer : IInitializer
{
    private readonly ILogger<DataBaseInitializer> _logger;
    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public DataBaseInitializer(ILogger<DataBaseInitializer> logger, ITibiaCharacterFinderDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public int? Order => 1;

    public async Task Initialize()
    {
        try
        {
            _logger.LogInformation("Database tibia initializer - started");
            await _dbContext.Database.MigrateAsync();
            _logger.LogInformation("Database tibia initializer - finished");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while migrating the database");
            Log.CloseAndFlush();
            throw;
        }
    }
}
