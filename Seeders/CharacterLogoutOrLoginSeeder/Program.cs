using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Providers;

namespace CharacterLogoutOrLoginSeeder
{
    public class Program
    {
        static void Main(string[] args)
        {

            var services = new ServiceCollection();

            ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var seeder = serviceProvider.GetService<CharacterLogoutOrLoginSeeder>();
            while (true)
            {
                seeder.Seed();
            }
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();
            services
                .AddSingleton<CharacterLogoutOrLoginSeeder>()
                .AddScoped<IDapperConnectionProvider, DapperConnectionProvider>()
                .AddSingleton<DbContextOptions<TibiaCharacterFinderDbContext>>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
            Startup.ConfigureOptions(services, configuration);
        }
    }
}
