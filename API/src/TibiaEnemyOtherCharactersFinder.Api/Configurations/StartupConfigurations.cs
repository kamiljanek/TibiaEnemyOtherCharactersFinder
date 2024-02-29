using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.RateLimiting;
using Serilog.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Policies;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services.BackgroundServices;

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
            diagnosticContext.Set("Origin", httpContext.Request.Headers.Origin);
            diagnosticContext.Set("RequestQuery", httpContext.Request.Query);
            diagnosticContext.Set("Cookies", httpContext.Request.Cookies);
            diagnosticContext.Set("SessionId", httpContext.Session.Id);
        };
    }

    public static Action<SwaggerUIOptions> SwaggerUiConfiguration(IApplicationBuilder app)
    {
        return options =>
        {
            var descriptionsProvider = app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

            // Build a swagger endpoint for each discovered API version
            foreach (var description in descriptionsProvider.ApiVersionDescriptions)
            {
                var url = $"/swagger/{description.GroupName}/swagger.json";
                var name = description.GroupName.ToUpperInvariant();
                options.SwaggerEndpoint(url, name);
                options.RoutePrefix = string.Empty;
            }
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

        services.AddOptions<BackgroundServiceTimerSection>()
            .BindConfiguration(BackgroundServiceTimerSection.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();
    }
}