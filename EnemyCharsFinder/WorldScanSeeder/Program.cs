using Microsoft.Extensions.DependencyInjection;
using System;
using TibiaCharacterFinderAPI.Entities;
using TibiaCharacterFinderAPI.Models;

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
            seeder.Seed();
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
