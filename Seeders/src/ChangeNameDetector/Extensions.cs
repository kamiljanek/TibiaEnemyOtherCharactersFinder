using Microsoft.Extensions.DependencyInjection;
using Shared.RabbitMQ.Conventions;
using Shared.RabbitMQ.Events;

namespace ChangeNameDetector;

public static class Extensions
{
    public static IServiceCollection AddIntegrationEvents(this IServiceCollection service)
    {
        service.AddSingleton(s => new EventBusSubscriberBuilder(s.GetRequiredService<IRabbitMqConventionProvider>()));
// UNDONE: możliwe że to w ogóle nie jest potrzebne

        return service;
    }
}