using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<TibiaCharacterFinderDbContext>(opt => opt
               .UseNpgsql(configuration.GetConnectionString(nameof(ConnectionStringsSection.PostgreSql)))
               //.UseNpgsql(configuration.GetConnectionString("PostgreSql"))
               // UNDONE: mozliwe ze linijka powyzej jest do wywalenia
               .UseSnakeCaseNamingConvention());

            return services;
        }
    }
}
