using Microsoft.Extensions.DependencyInjection;
using WorldSeeder.Decorators;

namespace WorldSeeder.Configuration;

public static class WorldSeederService
{
    public static IServiceCollection AddWorldSeeder(this IServiceCollection services)
    {
        services.AddScoped<IWorldSeederService, WorldSeeder.WorldSeederService>();
        services.AddScoped<IWorldService, WorldService>();
        services.AddScoped<IWorldSeederLogDecorator, WorldSeederLogDecorator>();

        return services;
    }
}