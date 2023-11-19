using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;

public static class CustomHostBuilder
{
    public static IHost Create(string projectName,
        Action<HostBuilderContext, IServiceCollection> configureServices,
        Action<ContainerBuilder> configureContainer = null)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddEnvironmentVariables();
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule<AutofacModule>();
                configureContainer?.Invoke(builder);
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddSerilog(context.Configuration, projectName)
                    .AddTibiaDbContext(context.Configuration);
                configureServices?.Invoke(context, services);
            })
            .UseSerilog()
            .Build();

        return host;
    }
}