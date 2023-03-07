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
            services.AddDbContext<TibiaCharacterFinderDbContext>(opt => opt
               .UseNpgsql(
                   configuration.GetConnectionString(nameof(ConnectionStringsSection.PostgreSql)), 
                   o => o.MinBatchSize(1)
                       .MaxBatchSize(30)
                       .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
               .UseSnakeCaseNamingConvention());

            return services;
        }
    }
}
