using CharacterAnalyser.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace CharacterAnalyser;

public class Program
{
    private static bool _hasDataToAnalyse = true;
    private static async Task Main(string[] args)
    {
        while (_hasDataToAnalyse)
        {
            await Analyse();
        }
    }

    private static async Task Analyse()
    {
        var services = new ServiceCollection();
        services
            .AddCharacterAnalyser()
            .AddServices()
            .AddTibiaDbContext();

        var serviceProvider = services.BuildContainer();
        var seeder = serviceProvider.GetService<IAnalyser>();

        _hasDataToAnalyse = await seeder.HasDataToAnalyse();
        if (!_hasDataToAnalyse)
            return;

        foreach (var worldId in seeder.UniqueWorldIds)
        {
            try
            {
                var worldScans = await seeder.GetWorldScansToAnalyseAsync(worldId);
                await seeder.Seed(worldScans);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
