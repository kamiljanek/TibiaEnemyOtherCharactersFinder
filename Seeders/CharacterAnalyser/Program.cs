using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var provider = ConfigureAppServices.Configure<ICharacterAnalyser, CharacterAnalyser>(services);
        var seeder = provider.GetService<ICharacterAnalyser>();

        await seeder.SetProperties();

        while (true)
        {
            await seeder.Seed();
        }
    }
}