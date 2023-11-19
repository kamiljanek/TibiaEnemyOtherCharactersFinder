using Microsoft.AspNetCore.RateLimiting;
using Serilog.AspNetCore;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;

namespace TibiaEnemyOtherCharactersFinder.Api.Configurations;

public static class StartupConfigurations
{
    public static void RateLimitedConfiguration(RateLimiterOptions options)
    {
        options.AddPolicy<string, GlobalRateLimiterPolicy>(ConfigurationConstants.GlobalRateLimiting);
        options.AddPolicy<string, PromptRateLimiterPolicy>(ConfigurationConstants.PromptRateLimiting);
    }

    public static void SerilogRequestLoggingConfiguration(RequestLoggingOptions configure)
    {
        configure.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} (UserId:'{UserId}') responded {StatusCode} in {Elapsed:0.0000}ms";
        configure.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("UserId", httpContext.Items["UserId"]);
            diagnosticContext.Set("RequestBody", httpContext.Items["Body"]);
            diagnosticContext.Set("Referer", httpContext.Request.Headers.Referer);
            diagnosticContext.Set("RequestQuery", httpContext.Request.Query);
            diagnosticContext.Set("Cookies", httpContext.Request.Cookies);
            diagnosticContext.Set("SessionId", httpContext.Session.Id);
        };
    }

    public static void ConfigureOptions(IServiceCollection services)
    {
        services.AddOptions<ConnectionStringsSection>()
            .BindConfiguration(ConnectionStringsSection.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<DapperConfigurationSection>()
            .BindConfiguration(DapperConfigurationSection.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}