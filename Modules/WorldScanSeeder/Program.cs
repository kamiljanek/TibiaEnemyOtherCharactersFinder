using Microsoft.Extensions.DependencyInjection;
using Shered.Services;
using TibiaEnemyOtherCharactersFinderApi.Entities;
using TibiaEnemyOtherCharactersFinderApi.Models;

namespace WorldScanSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<ISeeder>();
            while (true)
            {
                try
                {
                    seeder.Seed();
                    Console.WriteLine("Success" + DateTime.Now);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                System.Threading.Thread.Sleep(1000 * 60 * 5);
            }
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ISeeder, WorldScanSeeder>()
                .AddSingleton<Decompressor>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
