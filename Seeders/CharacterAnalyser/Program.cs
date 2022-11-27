using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Providers;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace CharacterAnalyserSeeder
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();
            var seeder = serviceProvider.GetService<CharacterActionSeeder>();

            while (true)
            {
                Console.WriteLine(DateTime.Now);
                seeder.Seed();
                Console.WriteLine(DateTime.Now);
            }
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();
            services
                .AddSingleton<CharacterActionSeeder>()
                .AddScoped<IDapperConnectionProvider, DapperConnectionProvider>()
                .AddSingleton<DbContextOptions<TibiaCharacterFinderDbContext>>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
            Startup.ConfigureOptions(services, configuration);
        }
    }
}
