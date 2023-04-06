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
                   options =>
                   {
                       options
                           .MinBatchSize(1)
                           .MaxBatchSize(30)
                           .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
                           .CommandTimeout(configuration
                               .GetSection($"{EfCoreConfigurationSection.SectionName}:{EfCoreConfigurationSection.CommandTimeout}")
                               .Get<int>());
                   })
               .UseSnakeCaseNamingConvention());

            return services;
        }
    }
}
