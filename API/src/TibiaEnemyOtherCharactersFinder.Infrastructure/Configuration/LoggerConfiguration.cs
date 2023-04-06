using Microsoft.Extensions.Configuration;
using Serilog;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class LoggerConfiguration
{
    public static void ConfigureLogger(IConfiguration configuration, string projectName)
    {
        var loggerConfiguration = new Serilog.LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ProjectName", projectName)
            .ReadFrom.Configuration(configuration);

        Log.Logger = loggerConfiguration.CreateLogger();
    }
    // UNDONE: dodać informacje o characterze podczas pobierania jego innych postaci (np. lvl, server itp)
    // UNDONE: jeżeli character nie został znaleziony lub ma pusty array wtedy zwrocic jakis komunikat
}