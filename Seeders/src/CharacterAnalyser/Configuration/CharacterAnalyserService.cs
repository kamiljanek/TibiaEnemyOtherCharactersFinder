using CharacterAnalyser.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterAnalyser.Configuration;

public static class CharacterAnalyserService
{
    public static IServiceCollection AddCharacterAnalyser(this IServiceCollection services)
    {
        services.AddScoped<IAnalyserService, AnalyserService>();
        services.AddScoped<IAnalyser, Analyser>();
        services.AddScoped<CharacterManager>();
        services.AddScoped<CharacterActionsCleaner>();
        services.AddScoped<CharacterCorrelationSeederService>();
        services.AddScoped<CharacterCorrelationUpdater>();
        services.AddScoped<CharacterCorrelationDeleter>();
        services.AddScoped<CharacterSeederService>();

        return services;
    }
}