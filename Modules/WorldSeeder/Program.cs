using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Providers;

namespace WorldSeeder
{
    public class Program
    {
        static async Task Main(string[] args)
        {

            var services = new ServiceCollection();

            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var seeder = serviceProvider.GetService<WorldSeeder>();
            await seeder.Seed();
            await seeder.TurnOffIfWorldIsUnavailable();
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();
            services
                .AddSingleton<WorldSeeder>()
                .AddHttpClient()
                .AddScoped<IDapperConnectionProvider, DapperConnectionProvider>()
                .AddSingleton<DbContextOptions<TibiaCharacterFinderDbContext>>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
            Startup.ConfigureOptions(services, configuration);
        }
    }
}
