using ChangeNameDetector.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Shared.RabbitMQ.Extensions;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;

namespace ChangeNameDetector;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var host = CustomHostBuilder.Create((context, services) =>
            {
                services.AddNameDetector();
                services.AddRabbitMqPublisher(context.Configuration);
            });

            Log.Information("Starting application");

            await host.StartAsync();
            var service = ActivatorUtilities.CreateInstance<ChangeNameDetectorService>(host.Services);
            await service.Run();
            await host.StopAsync();

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