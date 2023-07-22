using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.RabbitMQ.Initializers;

public class InitializationRabbitMqTaskRunner : IHostedService
{
    private readonly ILogger<InitializationRabbitMqTaskRunner> _logger;
    private readonly IServiceProvider _serviceProvider;

    public InitializationRabbitMqTaskRunner(
        IServiceProvider serviceProvider,
        ILogger<InitializationRabbitMqTaskRunner> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();

        var initializationTasks = scope.ServiceProvider.GetServices<IRabbitMqInitializer>().ToList();
        _logger.LogDebug("Found: {InitializationTasksCount} tasks to run", initializationTasks.Count);

        var timer = new Stopwatch();

        foreach (var task in initializationTasks)
        {
            var taskName = task.GetType().Name;
            timer.Restart();

            try
            {
                _logger.LogDebug("Running initialization task: {TaskName}", taskName);

                await task.InitializeAsync(cancellationToken);

                _logger.LogDebug("Task: {TaskName} initialized in {TimerElapsed}", taskName, timer.Elapsed);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Task: {TaskName} thrown exception {Message}", taskName, exception.Message);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}