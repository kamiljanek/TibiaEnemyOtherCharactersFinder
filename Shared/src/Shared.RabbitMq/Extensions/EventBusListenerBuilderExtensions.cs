using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;
using Shared.RabbitMQ.Handlers;

namespace Shared.RabbitMQ.Extensions;

public static class EventBusListenerBuilderExtensions
{
    public static SubscriptionAction<T> SubscribeEvent<T>(this EventBusSubscriberBuilder builder)
        where T : class, IIntegrationEvent
        => builder.SubscribeEvent(new EventBusIntegrationEventHandler<T>());
}