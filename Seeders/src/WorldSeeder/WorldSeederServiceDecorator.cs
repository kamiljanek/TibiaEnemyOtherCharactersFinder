using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace WorldSeeder;

public class WorldSeederServiceDecorator : ILoggerDecorator, IWorldSeederService
{
    private readonly ILogger<WorldSeederServiceDecorator> _logger;
    private readonly IWorldSeederService _worldSeederService;

    public WorldSeederServiceDecorator(ILogger<WorldSeederServiceDecorator> logger, IWorldSeederService worldSeederService)
    {
        _logger = logger;
        _worldSeederService = worldSeederService;
    }

    public async Task Seed()
    {
        await Decorate(_worldSeederService.Seed);
    }

    public async Task SetProperties()
    {
        await Decorate(_worldSeederService.SetProperties);
    }

    public async Task TurnOffIfWorldIsUnavailable()
    {
        await Decorate(_worldSeederService.TurnOffIfWorldIsUnavailable);
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