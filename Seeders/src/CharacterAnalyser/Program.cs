using System.Diagnostics;
using CharacterAnalyser.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace CharacterAnalyser;

public class Program
{
    private static async Task Main(string[] args)
    {
        var services = new ServiceCollection();

        services
            .AddCharacterAnalyser()
            .AddServices()
            .AddTibiaDbContext()
            .AddDapper();

        var serviceProvider = services.BuildContainer();

        var seeder = serviceProvider.GetService<IAnalyser>();
        
        Stopwatch sw = Stopwatch.StartNew();
        while (await seeder.HasDataToAnalyse())
        {
            foreach (var worldId in seeder.UniqueWorldIds)
            {
                var worldScans = await seeder.GetWorldScansToAnalyseAsync(worldId);
                await seeder.Seed(worldScans);
            }
        }
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds/1000);
    }
}