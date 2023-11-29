using CharacterAnalyser.Decorators;
using Microsoft.Extensions.DependencyInjection;

namespace CharacterAnalyser.Configuration;

public static class CharacterAnalyserService
{
    public static IServiceCollection AddCharacterAnalyser(this IServiceCollection services)
    {
        services.AddScoped<IAnalyserService, AnalyserService>();
        services.AddScoped<IAnalyser, Analyser>();
        services.AddScoped<IAnalyserLogDecorator, AnalyserLogDecorator>();

        return services;
    }
}