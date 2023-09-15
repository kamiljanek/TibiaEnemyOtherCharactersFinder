using Shared.RabbitMQ.EventBus;

namespace Shared.RabbitMq.EventBus;

public abstract class EventBusHandler<T> : EventBusHandler<T, ValueTuple>
{
}

public abstract class EventBusHandler<T, TResult>
{
    public virtual Func<IServiceProvider, string, Task<bool>> ShouldHandleAsync => (_, _) => Task.FromResult(true);

    public virtual Func<IServiceProvider, T, Task<TResult>> HandleAsync { get; } = (_, _) => default!;

    public virtual Func<IServiceProvider, EventBusHandleResult<TResult>, Task> HandleAfterAsync { get; } =
        (_, _) => Task.CompletedTask;
}