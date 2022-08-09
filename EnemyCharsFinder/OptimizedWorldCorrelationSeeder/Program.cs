using Microsoft.Extensions.DependencyInjection;
using System;
using TibiaCharFinderAPI.Entities;

namespace OptimizedWorldCorrelationSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<OptimizedWorldCorrelationSeeder>();
            seeder.Seed();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<OptimizedWorldCorrelationSeeder>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
