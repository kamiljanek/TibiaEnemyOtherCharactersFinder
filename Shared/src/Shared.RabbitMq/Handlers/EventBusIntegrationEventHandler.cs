using EkoProTech.Shared.RabbitMQ.EventBus;
using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ.Events;

namespace Shared.RabbitMQ.Handlers;

public class EventBusIntegrationEventHandler<T> : EventBusHandler<T> where T : class, IIntegrationEvent
{
    public override Func<IServiceProvider, T, Task<ValueTuple>> HandleAsync =>
        async (serviceProvider, integrationEvent) =>
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            await scope.ServiceProvider
                .GetRequiredService<IIntegrationEventHandler<T>>()
                .HandleAsync(integrationEvent);

            return ValueTuple.Create();
        };
}