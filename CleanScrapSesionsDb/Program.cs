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

            var dbContext = ServiceProvider.GetService<TibiaCharacterFinderDbContext>();
  
            dbContext.WorldCorrelations.Clear(); //change "table" before run
            dbContext.SaveChanges();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
