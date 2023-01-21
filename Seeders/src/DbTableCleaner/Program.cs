using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace DbTableCleaner;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var provider = ConfigureAppServices.Configure<ICleaner, Cleaner>(services);
        var seeder = provider.GetService<ICleaner>();
        
        await seeder.ClearDeletedWorldScans();
    }
}