using Microsoft.Extensions.DependencyInjection;

namespace WorldSeeder.Configuration;

public static class WorldSeederService
{
    public static IServiceCollection AddWorldSeeder(this IServiceCollection services)
    {
        services.AddScoped<IWorldSeederService, WorldSeeder.WorldSeederService>();
        services.AddScoped<IWorldService, WorldService>();
        services.AddScoped<WorldSeederServiceDecorator>();

        return services;
    }
}