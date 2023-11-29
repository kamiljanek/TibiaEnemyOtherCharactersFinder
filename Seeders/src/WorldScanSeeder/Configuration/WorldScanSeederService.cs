using Microsoft.Extensions.DependencyInjection;
using WorldScanSeeder.Decorators;

namespace WorldScanSeeder.Configuration;

public static class WorldScanSeederService
{
    public static IServiceCollection AddWorldScanSeederServices(this IServiceCollection services)
    {
        services.AddScoped<IScanSeeder, ScanSeeder>();
        services.AddScoped<IWorldScanService, WorldScanService>();
        services.AddScoped<IScanSeederLogDecorator, ScanSeederLogDecorator>();

        return services;
    }
}