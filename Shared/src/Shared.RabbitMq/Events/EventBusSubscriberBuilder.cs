using System.Collections.Concurrent;
using EkoProTech.Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.EventBus;

namespace Shared.RabbitMQ.Events;

public class EventBusSubscriberBuilder
{
    private readonly IRabbitMqConventionProvider _conventionsProvider;
    private readonly ConcurrentDictionary<QueueBinding, Action<IEventBusSubscriber>> _subscribers = new();

    public EventBusSubscriberBuilder(IRabbitMqConventionProvider conventionsProvider)
    {
        _conventionsProvider = conventionsProvider;
    }

    public SubscriptionAction<T> SubscribeEvent<T>(EventBusHandler<T> handler) where T : class, IIntegrationEvent
    {
       return new SubscriptionAction<T>(e => e.Subscribe(handler), this, _conventionsProvider);
    }

    public IEnumerable<QueueBinding> GetBindings() => _subscribers.Keys;

    internal void Subscribe(QueueBinding queueBinding, Action<IEventBusSubscriber> subscribeAction)
    {
        _subscribers.TryAdd(queueBinding, subscribeAction);
    }
}