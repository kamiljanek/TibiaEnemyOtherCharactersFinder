using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Providers;
using System;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace DbTableCleaner
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<TableCleaner>();
            
            await seeder.CleanWorldScansTable();
        }

        public static ServiceProvider ServiceProvider { get; private set; }

        private static void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();

            services
                .AddSingleton<TableCleaner>()
                .AddScoped<IDapperConnectionProvider, DapperConnectionProvider>();

            Startup.ConfigureOptions(services, configuration);
        }
    }
}
