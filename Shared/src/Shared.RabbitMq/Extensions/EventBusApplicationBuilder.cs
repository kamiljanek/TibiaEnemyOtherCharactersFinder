using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;

namespace Shared.RabbitMq.Extensions;

public class EventBusApplicationBuilder
{
    public void UseEventBus(IServiceProvider app)
    {
        var eventBusSubscriber = app.GetService<IEventBusSubscriber>();
        if (eventBusSubscriber == null)
        {
            return;
        }

        var messageBusSubscriberBuilder = app.GetRequiredService<EventBusSubscriberBuilder>();

        foreach (Action<IEventBusSubscriber> actionSubscribe in messageBusSubscriberBuilder.SubscribeActions)
        {
            actionSubscribe(eventBusSubscriber);
        }
    }
}