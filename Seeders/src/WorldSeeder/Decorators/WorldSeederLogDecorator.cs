using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace WorldSeeder.Decorators;

public class WorldSeederLogDecorator : IWorldSeederLogDecorator
{
    private readonly ILogger<WorldSeederLogDecorator> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;

    public WorldSeederLogDecorator(ILogger<WorldSeederLogDecorator> logger)
    {
        _logger = logger;
        _retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(3, _ => TimeSpan.FromMicroseconds(500));
    }


    public async Task Decorate(Func<Task> function)
    {
        var currentRetry = 0;

        await _retryPolicy.ExecuteAsync(async () =>
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                await function.Invoke();
                stopwatch.Stop();
                _logger.LogInformation("Method '{methodName}', execution time : {time} ms.", function.Method.Name,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception exception)
            {
                currentRetry++;
                _logger.LogError("Execution {methodName} couse error. Retry {retry}. Exception {exception}", function.Method.Name, currentRetry, exception);
            }
        });
    }
}