using Microsoft.Extensions.DependencyInjection;

namespace ChangeNameDetector.Configuration;

public static class NameDetectorService
{
    public static IServiceCollection AddNameDetector(this IServiceCollection services)
    {
        services.AddScoped<IChangeNameDetectorService, ChangeNameDetectorService>();
        services.AddScoped<CorrelationService>();

        return services;
    }
}