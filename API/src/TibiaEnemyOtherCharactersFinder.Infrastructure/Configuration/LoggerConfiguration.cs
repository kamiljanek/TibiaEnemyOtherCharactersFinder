using Microsoft.Extensions.Configuration;
using Serilog;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class LoggerConfiguration
{
    public static void ConfigureLogger(IConfiguration configuration)
    {
        var loggerConfiguration = new Serilog.LoggerConfiguration()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration);
            
        Log.Logger = loggerConfiguration.CreateLogger();
    }
}