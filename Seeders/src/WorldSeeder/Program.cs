using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;
using WorldSeeder.Configuration;

namespace WorldSeeder;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;

            var host = CustomHostBuilder.Create(
                projectName,
                (_, services) => { services.AddWorldSeeder(); });

            Log.Information("Starting application");

            var service = ActivatorUtilities.CreateInstance<WorldService>(host.Services);
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