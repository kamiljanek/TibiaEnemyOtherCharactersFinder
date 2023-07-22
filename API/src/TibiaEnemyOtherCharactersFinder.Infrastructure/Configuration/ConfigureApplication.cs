using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Clients.TibiaData;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureApplication
{
    public static IServiceCollection AddTibiaDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        return services;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, IConfiguration configuration, string projectName)
    {
        LoggerConfiguration.ConfigureLogger(configuration, projectName);
        return services;
    }

    // public static IServiceCollection AddCustomHttpClient(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddHttpClient<ITibiaDataClient, TibiaDataClient>("TibiaDataClient", client =>
    //     {
    //         client.BaseAddress = new Uri(configuration[$"{TibiaDataSection.SectionName}:{nameof(TibiaDataSection.BaseAddress)}"]);
    //         client.Timeout = TimeSpan.Parse(configuration[$"{TibiaDataSection.SectionName}:{nameof(TibiaDataSection.Timeout)}"]);
    //     });
    //
    //     return services;
    // }
    // UNDONE: do wywalenia
}