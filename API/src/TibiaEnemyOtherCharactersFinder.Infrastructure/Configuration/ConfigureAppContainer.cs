using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureAppContainer
{
    public static IContainer BuildContainerWithServicesAndOptions(IServiceCollection services)
    {
        var builder = new ContainerBuilder();

        builder.ConfigureOptions();
        builder.RegisterModule<AutofacModule>();
        builder.Populate(services);

        var container = builder.Build();

        return container;
    }
}