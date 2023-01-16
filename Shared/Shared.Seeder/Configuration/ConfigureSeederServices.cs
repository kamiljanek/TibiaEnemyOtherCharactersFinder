using Microsoft.Extensions.DependencyInjection;
using Shared.Configuration;
using Shared.Seeder.Configuration;

namespace Shared.Seeder.Configuration;

public static class ConfigureSeederServices
{
    public static IServiceCollection AddSeederServices(this IServiceCollection service)
    {
        service.AddOptions();
        return service;
    }
}