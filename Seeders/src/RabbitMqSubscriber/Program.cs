using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMqSubscriber.Configurations;
using RabbitMqSubscriber.Subscribers;
using Serilog;
using Shared.RabbitMQ.Extensions;
using Shared.RabbitMQ.Initializers;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;

namespace RabbitMqSubscriber;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var projectName = Assembly.GetExecutingAssembly().GetName().Name;

            var host = CustomHostBuilder.Create(
                projectName,
                (context, services) =>
                {
                    services.AddRabbitMqSubscriberServices();
                    services.AddRabbitMqSubscriber(context.Configuration);
                },
                builder => builder.RegisterEventSubscribers());

            Log.Information("Starting application");

            var initializer = ActivatorUtilities.CreateInstance<InitializationRabbitMqTaskRunner>(host.Services);
            await initializer.StartAsync();
            var service = ActivatorUtilities.CreateInstance<TibiaSubscriber>(host.Services);
            service.Subscribe();
            await host.WaitForShutdownAsync();

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
}