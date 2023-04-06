using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace WorldSeeder;

public class WorldSeederDecorator : ILoggerDecorator, IWorldSeeder
{
    private readonly ILogger<WorldSeederDecorator> _logger;
    private readonly IWorldSeeder _worldSeeder;

    public WorldSeederDecorator(ILogger<WorldSeederDecorator> logger, IWorldSeeder worldSeeder)
    {
        _logger = logger;
        _worldSeeder = worldSeeder;
    }

    public async Task Seed()
    {
        await Decorate(_worldSeeder.Seed);
    }

    public async Task SetProperties()
    {
        await Decorate(_worldSeeder.SetProperties);
    }

    public async Task TurnOffIfWorldIsUnavailable()
    {
        await Decorate(_worldSeeder.TurnOffIfWorldIsUnavailable);
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