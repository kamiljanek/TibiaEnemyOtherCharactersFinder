using Microsoft.Extensions.DependencyInjection;

namespace WorldScanSeeder.Configuration;

public static class WorldScanSeederService
{
    public static IServiceCollection AddWorldScanSeeder(this IServiceCollection services)
    {
        services.AddScoped<IScanSeeder, ScanSeeder>();

        return services;
    }
}