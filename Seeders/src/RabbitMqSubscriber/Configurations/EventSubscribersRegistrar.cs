using System.Reflection;
using Autofac;
using RabbitMqSubscriber.Subscribers;

namespace RabbitMqSubscriber.Configurations;

public static class EventSubscribersRegistrar
{
    public static ContainerBuilder RegisterEventSubscribers(this ContainerBuilder builder)
    {
        var eventSubscriberTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(IEventSubscriber).IsAssignableFrom(type) && !type.IsInterface).ToList();
        foreach (var eventSubscriberType in eventSubscriberTypes)
        {
            builder.RegisterType(eventSubscriberType).As<IEventSubscriber>().SingleInstance();
        }

        return builder;
    }
}