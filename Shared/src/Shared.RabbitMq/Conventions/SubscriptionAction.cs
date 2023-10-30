using Shared.RabbitMQ.EventBus;
using Shared.RabbitMQ.Events;

namespace Shared.RabbitMQ.Conventions;

public class SubscriptionAction<T>
{
    private readonly Action<IEventBusSubscriber> _subscribeAction;
    private readonly EventBusSubscriberBuilder _builder;
    private readonly IRabbitMqConventionProvider _conventionProvider;

    public SubscriptionAction(Action<IEventBusSubscriber> subscribeAction,
        EventBusSubscriberBuilder builder,
        IRabbitMqConventionProvider conventionProvider)
    {
        _subscribeAction = subscribeAction;
        _builder = builder;
        _conventionProvider = conventionProvider;
    }

    public EventBusSubscriberBuilder AsSelf()
    {
        QueueBinding queueBinding = _conventionProvider.GetForType<T>();
        _builder.Subscribe(queueBinding, _subscribeAction);

        return _builder;
    }
}