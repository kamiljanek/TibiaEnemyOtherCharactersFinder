using Microsoft.Extensions.DependencyInjection;
using RabbitMqSubscriber.Handlers;
using RabbitMqSubscriber.Subscribers;

namespace RabbitMqSubscriber.Configurations;

public static class RabbitMqSubscriberService
{
    public static IServiceCollection AddRabbitMqSubscriberServices(this IServiceCollection services)
    {
        services.AddSingleton<TibiaSubscriber>();
        services.AddSingleton<IEventResultHandler, EventResultHandler>();

        return services;
    }
}