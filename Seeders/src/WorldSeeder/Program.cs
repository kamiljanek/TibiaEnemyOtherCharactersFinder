using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using WorldSeeder.Configuration;

namespace WorldSeeder;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services
            .AddWorldSeeder()
            .AddServices()
            .AddTibiaDbContext();

        var serviceProvider = services.BuildContainer();

        var seeder = serviceProvider.GetService<IWorldSeeder>();

        await seeder.SetProperties();
        await seeder.Seed();
        await seeder.TurnOffIfWorldIsUnavailable();
    }
}