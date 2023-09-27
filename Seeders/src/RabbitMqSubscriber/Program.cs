using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMqSubscriber.Configurations;
using RabbitMqSubscriber.Subscribers;
using Serilog;
using Shared.RabbitMQ.Extensions;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Builders;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace RabbitMqSubscriber;

public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var host = CustomHostBuilder.Create((context, services) =>
            {
                services.AddSingleton<TibiaSubscriber>();
                services.AddRabbitMqSubscriber(context.Configuration);
            }, builder => builder.RegisterEventSubscribers());

            Log.Information("Starting application");

            await host.StartAsync();
            var service = ActivatorUtilities.CreateInstance<TibiaSubscriber>(host.Services);
            service.Subscribe();
            await host.StopAsync();

            Console.ReadKey();

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