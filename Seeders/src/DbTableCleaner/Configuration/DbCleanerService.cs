using Microsoft.Extensions.DependencyInjection;

namespace DbCleaner.Configuration;

public static class DbCleanerService
{
    public static IServiceCollection AddDbCleaner(this IServiceCollection services)
    {
        services.AddScoped<ICleaner, Cleaner>();
        services.AddScoped<ICleanerService, CleanerService>();
        services.AddScoped<CleanerDecorator>();

        return services;
    }
}