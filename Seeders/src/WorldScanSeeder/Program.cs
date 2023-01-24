using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using WorldScanSeeder.Configuration;

namespace WorldScanSeeder;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services
            .AddWorldScanSeeder()
            .AddServices()
            .AddTibiaDbContext();

        var serviceProvider = services.BuildContainer();

        var seeder = serviceProvider.GetService<IWorldScanSeeder>();

        await seeder.SetProperties();
        foreach (var availableWorld in seeder.AvailableWorlds)
        {
            await seeder.Seed(availableWorld);
        }
    }
}