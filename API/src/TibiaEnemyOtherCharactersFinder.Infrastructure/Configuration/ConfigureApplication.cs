using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureApplication
{
    private static bool _wasAddDapperMethodCalled;

    public static IServiceProvider BuildContainer(this IServiceCollection services)
    {
        var containerBuilder = new ContainerBuilder();

        if (_wasAddDapperMethodCalled)
            containerBuilder.ConfigureOptions();

        _wasAddDapperMethodCalled = false;

        containerBuilder.RegisterModule<AutofacModule>();
        containerBuilder.Populate(services);

        var container = containerBuilder.Build();
        return new AutofacServiceProvider(container);
    }

    public static IServiceCollection AddDapper(this IServiceCollection services)
    {
        _wasAddDapperMethodCalled = true;
        return services;
    }

    public static IServiceCollection AddTibiaDbContext(this IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(ApplicationSettingsSections.FileName, optional: false, reloadOnChange: true).Build();

        services.AddInfrastructure(configuration);

        return services;
    }

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