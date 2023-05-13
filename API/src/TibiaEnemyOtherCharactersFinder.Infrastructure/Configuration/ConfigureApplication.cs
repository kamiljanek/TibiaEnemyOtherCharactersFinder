using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHttpClient();

        return services;
    }
}