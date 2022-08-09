using Microsoft.Extensions.DependencyInjection;
using TibiaCharacterFinderAPI.Entities;
using TibiaCharacterFinderAPI.Models;

namespace WorldSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<WorldSeeder>();
            seeder.Seed();
            seeder.TurnOffIfWorldIsUnavailable();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<ISeeder, WorldSeeder>()
                .AddSingleton<Decompressor>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
