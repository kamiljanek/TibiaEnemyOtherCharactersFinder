using System;
using Microsoft.Extensions.DependencyInjection;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace WorldCorrelationSeeder
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
                .AddSingleton<ISeeder, WorldCorrelationSeeder>()
                .AddSingleton<Decompressor>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
