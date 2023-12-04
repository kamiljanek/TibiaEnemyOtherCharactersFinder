using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Shared.RabbitMQ.Initializers;

public sealed class InitializationRabbitMqTaskRunner
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

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        var initializationTask = scope.ServiceProvider.GetRequiredService<IRabbitMqInitializer>();
        _logger.LogInformation("Found: {InitializationTasksCount} tasks to run", initializationTask);

        var timer = new Stopwatch();

        var taskName = initializationTask.GetType().Name;
        timer.Restart();

        try
        {
            _logger.LogInformation("Running initialization task: {TaskName}", taskName);
            await initializationTask.InitializeAsync(cancellationToken);
            _logger.LogInformation("Task: {TaskName} initialized in {TimerElapsed}", taskName, timer.Elapsed);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Task: {TaskName} thrown exception {Message}", taskName, exception.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}