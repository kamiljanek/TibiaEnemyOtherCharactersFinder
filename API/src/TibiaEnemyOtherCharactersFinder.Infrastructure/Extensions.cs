using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Hubs;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Traceability;

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
                                .GetSection(
                                    $"{EfCoreConfigurationSection.SectionName}:{EfCoreConfigurationSection.CommandTimeout}")
                                .Get<int>());
                    })
                .UseSnakeCaseNamingConvention());

            services.AddOptions<TibiaDataSection>()
                .Bind(configuration.GetSection(TibiaDataSection.SectionName))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddHttpClient<ITibiaDataClient, TibiaDataClient>("TibiaDataClient", httpClient =>
                {
                    httpClient.BaseAddress =
                        new Uri(configuration[
                            $"{TibiaDataSection.SectionName}:{nameof(TibiaDataSection.BaseAddress)}"]);
                    httpClient.Timeout =
                        TimeSpan.Parse(
                            configuration[$"{TibiaDataSection.SectionName}:{nameof(TibiaDataSection.Timeout)}"]);
                })
                .AddHttpMessageHandler<HttpClientDecompressionHandler>()
                .AddPolicyHandler(CommunicationPolicies.GetTibiaDataRetryPolicy());

            return services;
        }

        public static void UseSignalrHubs(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<CharactersTrackHub>(HubRoutes.CharactersTrackHub, options =>
                {
                    options.CloseOnAuthenticationExpiration = true;
                });
            });
        }
    }
}
