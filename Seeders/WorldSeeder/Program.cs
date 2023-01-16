using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace WorldSeeder;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var provider = ConfigureAppServices.Configure<IWorldSeeder, WorldSeeder>(services);
        var seeder = provider.GetService<IWorldSeeder>();

        await seeder.SetProperties();
        await seeder.Seed();
        await seeder.TurnOffIfWorldIsUnavailable();
    }
}