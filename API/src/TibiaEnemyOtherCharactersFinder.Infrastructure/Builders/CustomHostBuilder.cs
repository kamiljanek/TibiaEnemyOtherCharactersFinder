﻿using System.Reflection;
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
    public static IHost Create(
        Action<HostBuilderContext, IServiceCollection> configureServices,
        Action<ContainerBuilder> configureContainer = null)
    {
        var host = Host.CreateDefaultBuilder()
            // .ConfigureAppConfiguration((hostingContext, config) =>
            // {
            //     config
            //         .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            //         .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            //         .AddEnvironmentVariables();
            // })

            // UNDONE: remove when application on prod will work properly
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
                    .AddSerilog(context.Configuration, Assembly.GetExecutingAssembly().GetName().Name)
                    .AddTibiaDbContext(context.Configuration);
                configureServices?.Invoke(context, services);
            })
            .UseSerilog()
            .Build();

        return host;
    }
}