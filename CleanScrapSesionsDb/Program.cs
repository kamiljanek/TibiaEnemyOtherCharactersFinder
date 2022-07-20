using Microsoft.Extensions.DependencyInjection;
using TibiaCharFinderAPI.Entities;

namespace CleanScrapSesionsDb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<CleanTable>();
            var context = ServiceProvider.GetService<TibiaCharacterFinderDbContext>();
  
            var tableName = context.WorldCorrelations; //choose table before run
            
            seeder.Clean(tableName);
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<CleanTable>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
