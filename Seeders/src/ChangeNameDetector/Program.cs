using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ChangeNameDetector.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Shared.RabbitMQ;
using Shared.RabbitMQ.Extensions;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace ChangeNameDetector;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args);

            Log.Information("Starting application");

            var service = ActivatorUtilities.CreateInstance<ChangeNameDetectorService>(host.Services);
            await service.Run();

            Log.Information("Ending application properly");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static IHost CreateHostBuilder(string [] args)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // config.Properties["reloadConfigOnChange"] = true;
                config
                    // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    // .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule<AutofacModule>();
            })
            .ConfigureServices((context, services) =>
            {
                services
                    .AddNameDetector()
                    .AddSerilog(context.Configuration, Assembly.GetExecutingAssembly().GetName().Name)
                    .AddTibiaDbContext(context.Configuration)
                    .AddIntegrationEvents()
                    .AddRabbitMqPublisher(context.Configuration);
            })
            .UseSerilog()
            .Build();

        return host;
        // UNDONE: wyeksportowac ta metode do shared
    }
}