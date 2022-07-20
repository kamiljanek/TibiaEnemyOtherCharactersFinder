using Microsoft.Extensions.DependencyInjection;
using System;
using TibiaCharFinderAPI.Entities;

namespace WorldScanSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<WorldScanSeeder>();
            seeder.Seed();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<WorldScanSeeder>()
                .AddSingleton<Decompressor>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
