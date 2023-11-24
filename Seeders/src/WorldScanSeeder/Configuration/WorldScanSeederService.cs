using Microsoft.Extensions.DependencyInjection;

namespace WorldScanSeeder.Configuration;

public static class WorldScanSeederService
{
    public static IServiceCollection AddWorldScanSeederServices(this IServiceCollection services)
    {
        services.AddScoped<IScanSeeder, ScanSeeder>();
        services.AddScoped<IWorldScanService, WorldScanService>();
        services.AddScoped<WorldScanSeederDecorator>();

        return services;
    }
}