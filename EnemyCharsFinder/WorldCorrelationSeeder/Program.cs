using System;
using Microsoft.Extensions.DependencyInjection;
using TibiaCharFinder.Entities;

namespace WorldCorrelationSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<WorldCorrelationSeeder>();
            seeder.Seed();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<WorldCorrelationSeeder>()
                .AddSingleton<Decompressor>()
                .AddSingleton<EnemyCharFinderDbContext>();
        }
    }
}
