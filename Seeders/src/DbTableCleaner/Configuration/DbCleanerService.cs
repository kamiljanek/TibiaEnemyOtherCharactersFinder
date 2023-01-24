using Microsoft.Extensions.DependencyInjection;

namespace DbCleaner.Configuration;

public static class DbCleanerService
{
    public static IServiceCollection AddDbCleaner(this IServiceCollection services)
    {
        services.AddScoped<ICleaner, Cleaner>();

        return services;
    }
}