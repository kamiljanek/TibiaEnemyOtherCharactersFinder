using Microsoft.Extensions.DependencyInjection;
using TibiaCharFinder.Entities;

namespace CleanScrapSesionsDb
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<CleanScrapSesion>();
            seeder.Clean();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<CleanScrapSesion>()
                .AddSingleton<EnemyCharFinderDbContext>();
        }
    }
}
