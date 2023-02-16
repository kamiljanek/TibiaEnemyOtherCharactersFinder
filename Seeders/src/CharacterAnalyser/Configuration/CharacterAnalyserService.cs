using CharacterAnalyser.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterAnalyser.Configuration;

public static class CharacterAnalyserService
{
    public static IServiceCollection AddCharacterAnalyser(this IServiceCollection services)
    {
        services.AddScoped<IAnalyser, Analyser>();
        services.AddScoped<CharacterActionSeeder>();
        services.AddScoped<CharacterAnalyserCleaner>();
        services.AddScoped<CharacterCorrelationSeeder>();
        services.AddScoped<CharacterSeeder>();

        return services;
    }
}