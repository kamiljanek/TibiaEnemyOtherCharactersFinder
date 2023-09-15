using ChangeNameDetector.Services;
using ChangeNameDetector.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeNameDetector.Configuration;

public static class NameDetectorService
{
    public static IServiceCollection AddNameDetector(this IServiceCollection services)
    {
        services.AddScoped<IChangeNameDetectorService, ChangeNameDetectorService>();
        services.AddSingleton<NameDetectorValidator>();

        return services;
    }
}