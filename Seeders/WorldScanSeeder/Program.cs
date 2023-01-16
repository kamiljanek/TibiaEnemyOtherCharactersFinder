using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldScanSeeder;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var provider = ConfigureAppServices.Configure<ISeeder, WorldScanSeeder>(services);
        var seeder = provider.GetService<ISeeder>();

        await seeder.Seed();
    }
}