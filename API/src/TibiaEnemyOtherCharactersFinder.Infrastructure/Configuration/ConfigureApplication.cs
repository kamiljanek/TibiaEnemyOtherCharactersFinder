using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureApplication
{
    // UNDONE: zmienić nazwę klasy
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
        var configuration = new ConfigurationBuilder().AddJsonFile(ApplicationSettingsSections.FileName, optional: false, reloadOnChange: true).Build();

        services.AddInfrastructure(configuration);

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        // UNDONE: mozliwe ze AddOptions można wywalić
        //services.AddOptions();
        services.AddHttpClient();

        return services;
    }
}