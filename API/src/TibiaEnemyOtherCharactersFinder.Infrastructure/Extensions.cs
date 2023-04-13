using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaDataApi;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

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

            services.AddOptions<TibiaDataApiSection>()
                .Bind(configuration.GetSection(TibiaDataApiSection.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient<ITibiaDataApiClient, TibiaDataApiClient>("TibiaDataApiClient", httpClient =>
                {
                    httpClient.BaseAddress = new Uri(configuration[$"{TibiaDataApiSection.SectionName}:{nameof(TibiaDataApiSection.BaseAddress)}"]);
                    httpClient.Timeout = TimeSpan.Parse(configuration[$"{TibiaDataApiSection.SectionName}:{nameof(TibiaDataApiSection.Timeout)}"]);
                });

            return services;
        }
    }
}
