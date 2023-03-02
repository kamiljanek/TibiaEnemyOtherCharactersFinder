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
        
        var sw = Stopwatch.StartNew();
        while (await seeder.HasDataToAnalyse())
        {
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

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds/1000);
    }
}