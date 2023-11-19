using System.Reflection;
using DbCleaner.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;

namespace DbCleaner;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;

            var host = CustomHostBuilder.Create(
                projectName,
                (_, services) => { services.AddDbCleaner(); });

            Log.Information("Starting application");

            var service = ActivatorUtilities.CreateInstance<CleanerService>(host.Services);
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