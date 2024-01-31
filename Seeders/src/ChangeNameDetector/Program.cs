using System.Reflection;
using ChangeNameDetector.Configuration;
using ChangeNameDetector.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.RabbitMQ.Extensions;
using Shared.RabbitMQ.Initializers;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;

namespace ChangeNameDetector;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;

            var host = CustomHostBuilder.Create(
                projectName,
                (context, services) =>
                {
                    services.AddNameDetector();
                    services.AddRabbitMqPublisher(context.Configuration);
                });

            Log.Information("Starting application");

            var initializer = ActivatorUtilities.CreateInstance<InitializationRabbitMqTaskRunner>(host.Services);
            await initializer.StartAsync();
            var service = ActivatorUtilities.CreateInstance<ChangeNameDetectorService>(host.Services);
            await service.Run();

            Log.Information("Ending application properly");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }
}