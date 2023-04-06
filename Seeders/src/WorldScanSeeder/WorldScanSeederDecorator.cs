using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace WorldScanSeeder;

public class WorldScanSeederDecorator : ILoggerDecorator, IScanSeeder
{
    private readonly ILogger<WorldScanSeederDecorator> _logger;
    private readonly IScanSeeder _scanSeeder;
    private List<World> _availableWorlds;

    public List<World> AvailableWorlds => _availableWorlds;

    public WorldScanSeederDecorator(ILogger<WorldScanSeederDecorator> logger, IScanSeeder scanSeeder)
    {
        _logger = logger;
        _scanSeeder = scanSeeder;
    }

    public async Task SetProperties()
    {
        await Decorate(_scanSeeder.SetProperties);
        _availableWorlds = _scanSeeder.AvailableWorlds;
    }

    public async Task Seed(World entity)
    {
        await Decorate(_scanSeeder.Seed, entity);
    }

    public async Task Decorate<T>(Func<T,Task> function, T parameter)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await function.Invoke(parameter);
            stopwatch.Stop();
            _logger.LogInformation("WorldId({worldId}) - {worldName} - Execution time {methodName}: {time} ms.",
                (parameter as World)!.WorldId, (parameter as World)!.Name, function.Method.Name, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "World Id({WorldId}) - Execution {methodName} couse error", (parameter as World)!.WorldId, function.Method.Name);
        }
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
}