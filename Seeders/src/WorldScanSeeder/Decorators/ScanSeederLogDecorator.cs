using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace WorldScanSeeder.Decorators;

public class ScanSeederLogDecorator : IScanSeederLogDecorator
{
    private readonly ILogger<ScanSeederLogDecorator> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public ScanSeederLogDecorator(ILogger<ScanSeederLogDecorator> logger)
    {
        _logger = logger;
        _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3, _ => TimeSpan.FromMicroseconds(500));
    }

    public async Task Decorate(Func<World, Task> function, World parameter)
    {
        var currentRetry = 0;

        await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await function.Invoke(parameter);
                stopwatch.Stop();
                _logger.LogInformation(
                    "WorldId({worldId}) - {worldName} - Method '{methodName}', execution time: {time} ms.",
                    parameter.WorldId, parameter.Name, function.Method.Name, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                currentRetry++;
                _logger.LogError("WorldId({WorldId}) - Execution {methodName} couse error. Retry {retry}. Exception {exception}",
                    parameter.WorldId, function.Method.Name, currentRetry, exception);
            }
        });
    }
}