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
using Shared.RabbitMq.Extensions;
using Shared.RabbitMQ.Initializers;

namespace Shared.RabbitMQ.Extensions;

public static class RabbitMqExtension
{
    public static void AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionName = "tibia-eocf-publisher";

        services.AddRabbitMqCommonSettings(configuration, connectionName)
            .AddSingleton<IEventBusPublisher, RabbitMqBusPublisher>(); // UNDONE: zmienić nazwę na RabbitMqPublisher oraz IEventPublisher
    }

    public static void AddRabbitMqSubscriber(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionName = "tibia-eocf-subscriber";

        services.AddRabbitMqCommonSettings(configuration, connectionName);
        // .AddSingleton<IEventBusSubscriber, RabbitMqBusSubscriber>();
    }

    private static IServiceCollection AddRabbitMqCommonSettings(this IServiceCollection services, IConfiguration configuration, string connectionName)
    {
        var section = configuration.GetSection(RabbitMqSection.SectionName);
        services.AddOptions<RabbitMqSection>()
            .Bind(section)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var options = section.Get<RabbitMqSection>();
        var factory = new ConnectionFactory
        {
            Uri = new Uri(options!.HostUrl),
            Port = options.Port,
            VirtualHost = options.VirtualHost,
            UserName = options.Username,
            Password = options.Password,
            DispatchConsumersAsync = true
// UNDONE: sprawdzić co tu dodać a co odjąć
        };

        try
        {
            IConnection rabbitMqConnection = factory.CreateConnection(connectionName);

            services
                .AddEvents() // UNDONE: to jest raczej nie potrzebne
                .AddSingleton<IRabbitMqConventionProvider, RabbitMqConventionProvider>()
                .AddSingleton(new RabbitMqConnection(rabbitMqConnection))
                .AddTransient<IRabbitMqInitializer, RabbitMqInitializer>()
                .AddSingleton<MessageSerializer>()
                .AddSingleton<EventBusApplicationBuilder>()
                .AddHostedService<InitializationRabbitMqTaskRunner>();
        }
        catch (BrokerUnreachableException ex)
        {
            Log.Warning("RabbitMq connection is closed. Error message: {Message}", ex.Message);
            Log.Warning("RabbitMq configuration: {RabbitConfig}", JsonSerializer.Serialize(options));
            // UNDONE: zastanowić się czy powyższa linijka jest potrzebna, bo tam zwraca mi hasło
        }

        return services;
    }

    private static IServiceCollection AddEvents(this IServiceCollection service)
    {
        service.AddSingleton(s =>
            new EventBusSubscriberBuilder(s.GetRequiredService<IRabbitMqConventionProvider>())
                .SubscribeEvent<MergeTwoCharactersEvent>().AsSelf()
                .SubscribeEvent<DeleteCharacterCorrelationsEvent>().AsSelf()
                .SubscribeEvent<DeleteCharacterWithCorrelationsEvent>().AsSelf());

        return service;
    }
}