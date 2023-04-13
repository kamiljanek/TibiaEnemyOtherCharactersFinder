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
}