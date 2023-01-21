using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureAppServices
{
    public static IServiceProvider Configure<TService, TImplementation>(IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TService, TImplementation>();
        services.AddServices();

        var container = ConfigureAppContainer.BuildContainerWithServicesAndOptions(services);

        return new AutofacServiceProvider(container);
    }

    private static IServiceCollection AddServices(this IServiceCollection service)
    {
        service
            .AddOptions()
            .AddHttpClient();
        
        return service;
    }
}