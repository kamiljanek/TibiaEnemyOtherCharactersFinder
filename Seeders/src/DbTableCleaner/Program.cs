using DbCleaner.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace DbCleaner;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        services
            .AddDbCleaner()
            .AddServices()
            .AddDapper();

        var serviceProvider = services.BuildContainer();

        var seeder = serviceProvider.GetService<ICleaner>();

        await seeder.ClearDeletedWorldScans();
    }
}