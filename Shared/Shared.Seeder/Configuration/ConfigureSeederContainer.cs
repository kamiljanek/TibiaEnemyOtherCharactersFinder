using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Shared.Seeder.Configuration;

public static class ConfigureSeederContainer
{
    public static ContainerBuilder Configure(IServiceCollection services)
    {
        var builder = new ContainerBuilder();
        builder
            .AddServices(services)
            .AddOptions();

        builder.RegisterModule<AutofacModule>();


        return builder;
    }

    private static ContainerBuilder AddServices(this ContainerBuilder builder, IServiceCollection services)
    {
        builder.Populate(services);

        return builder;
    }

    private static ContainerBuilder AddOptions(this ContainerBuilder builder)
    {
        builder.ConfigureOptions();

        return builder;
    }
}