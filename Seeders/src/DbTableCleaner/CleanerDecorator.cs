using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace DbCleaner;

public class CleanerDecorator : ILoggerDecorator
{
    private readonly ILogger<CleanerDecorator> _logger;
    private readonly ICleaner _cleaner;

    public CleanerDecorator(ILogger<CleanerDecorator> logger, ICleaner cleaner)
    {
        _logger = logger;
        _cleaner = cleaner;
    }

    public async Task ClearTables()
    {
        await Decorate(_cleaner.ClearDeletedWorldScans);
        await Decorate(_cleaner.TruncateCharacterActions);
        await Decorate(_cleaner.DeleteIrrelevantCharacterCorrelations);
        await Decorate(_cleaner.VacuumCharacterActions);
        await Decorate(_cleaner.VacuumWorldScans);
        await Decorate(_cleaner.VacuumCharacters);
        await Decorate(_cleaner.VacuumWorlds);
        await Decorate(_cleaner.VacuumCharacterCorrelations);
    }

    public async Task Decorate(Func<Task> function)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await function.Invoke();
            stopwatch.Stop();
            _logger.LogInformation("Execution time {methodName}: {time} ms.", function.Method.Name,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Execution {methodName} couse error", function.Method.Name);
        }
    }

    public Task Decorate<T>(Func<T, Task> function, T parameter)
    {
        throw new NotImplementedException();
    }
}