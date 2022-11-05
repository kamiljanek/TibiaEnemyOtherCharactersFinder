using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shered.Services;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Api.Entities;
using TibiaEnemyOtherCharactersFinder.Api.Models;
using TibiaEnemyOtherCharactersFinder.Api.Providers;

namespace WorldScanSeeder
{
    public class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<ISeeder>();

            try
            {
                seeder.Seed();
                Console.WriteLine("Success" + DateTime.Now);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static ServiceProvider ServiceProvider { get; private set; }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();

            services
                .AddSingleton<ISeeder, WorldScanSeeder>()
                .AddSingleton<Decompressor>()
                .AddScoped<IDapperConnectionProvider, DapperConnectionProvider>()
                .AddSingleton<DbContextOptions<TibiaCharacterFinderDbContext>>()
                .AddSingleton<TibiaCharacterFinderDbContext>();

            Startup.ConfigureOptions(services, configuration);
        }
    }
}
