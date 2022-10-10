using Microsoft.Extensions.DependencyInjection;
using System;
using TibiaCharacterFinderAPI.Entities;

namespace CharacterLogoutOrLoginSeeder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            var seeder = ServiceProvider.GetService<CharacterLogoutOrLoginSeeder>();
            seeder.Seed();
        }
        public static ServiceProvider ServiceProvider { get; private set; }
        private static void ConfigureServices(IServiceCollection services)
        {
            services
                .AddSingleton<CharacterLogoutOrLoginSeeder>()
                .AddSingleton<TibiaCharacterFinderDbContext>();
        }
    }
}
