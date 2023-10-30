using CharacterAnalyser.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;

namespace CharacterAnalyser;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var host = CustomHostBuilder.Create((_, services) =>
            {
                services.AddCharacterAnalyser();
            });

            Log.Information("Starting application");

            var service = ActivatorUtilities.CreateInstance<AnalyserService>(host.Services);
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
