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
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
            //services.AddSingleton<IConfiguration>(configuration);
            services
                //.AddSingleton<Startup>()
                .AddSingleton<CharacterLogoutOrLoginSeeder>()
                .AddScoped<IDapperConnectionProvider, DapperConnectionProvider>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
            Startup.ConfigureOptions(services, configuration);
            //services.Configure<ConnectionStringsSection>(opt => configuration.GetSection("ConnectionStrings").Bind(opt));
            //services.Configure<DapperConfigurationSection>(options => configuration.GetSection("Dapper").Bind(options));
        }
    }
}
