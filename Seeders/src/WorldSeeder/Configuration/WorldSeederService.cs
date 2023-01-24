using Microsoft.Extensions.DependencyInjection;

namespace WorldSeeder.Configuration;

public static class WorldSeederService
{
    public static IServiceCollection AddWorldSeeder(this IServiceCollection services)
    {
        services.AddScoped<IWorldSeeder, WorldSeeder>();

        return services;
    }
}