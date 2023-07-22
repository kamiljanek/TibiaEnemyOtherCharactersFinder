using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using Serilog;
using Shared.RabbitMQ.Configuration;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;
using Shared.RabbitMQ.Initializers;

namespace Shared.RabbitMQ.Extensions;

public static class RabbitMqExtension
{
    public static void AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionName = "tibia-eocf-publisher";

        services.AddRabbitMqCommonSettings(configuration, connectionName, out var rabbitMqConnection)
            .AddSingleton(new PublisherConnection(rabbitMqConnection))
            .AddSingleton<IEventBusPublisher, RabbitMqBusPublisher>();
    }

    public static void AddRabbitMqSubscriber(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionName = "tibia-eocf-subscriber";

        services.AddRabbitMqCommonSettings(configuration, connectionName, out var rabbitMqConnection)
            .AddSingleton(new SubscriberConnection(rabbitMqConnection))
            .AddSingleton<IEventBusSubscriber, RabbitMqBusSubscriber>();
    }

    private static IServiceCollection AddRabbitMqCommonSettings(this IServiceCollection services,
        IConfiguration configuration, string connectionName, out IConnection connection)
    {
        var section = configuration.GetSection(RabbitMqSection.SectionName);
        services.AddOptions<RabbitMqSection>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var options = section.Get<RabbitMqSection>();
        var factory = new ConnectionFactory
        {
            Uri = new Uri(options.HostUrl),
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.Username,
            Password = options.Password,
            DispatchConsumersAsync = true,
// UNDONE: sprawdzić co tu dodać a co odjąć
        };

        IConnection rabbitMqConnection = default!;
        try
        {
            rabbitMqConnection =
                factory.CreateConnection(connectionName);

            services
                .AddTransient<IRabbitMqInitializer, RabbitMqInitializer>()
                .AddHostedService<InitializationRabbitMqTaskRunner>()
                .AddSingleton<MessageSerializer>()
                .AddSingleton<IRabbitMqConventionProvider, RabbitMqConventionProvider>();
        }
        catch (BrokerUnreachableException ex)
        {
            Log.Warning("RabbitMq connection is closed. Error message: {Message}", ex.Message);
            Log.Warning("RabbitMq configuration: {RabbitConfig}", JsonSerializer.Serialize(options));
        }

        connection = rabbitMqConnection;
        return services;
    }
}