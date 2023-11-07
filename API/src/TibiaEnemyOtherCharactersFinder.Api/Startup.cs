using Autofac;
using MediatR;
using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared.RabbitMQ;
using Swashbuckle.AspNetCore.SwaggerGen;
using TibiaEnemyOtherCharactersFinder.Api.Swagger;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace TibiaEnemyOtherCharactersFinder.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        Infrastructure.Configuration.LoggerConfiguration.ConfigureLogger(_configuration,
            Assembly.GetExecutingAssembly().GetName().Name);
    }

    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule<AutofacModule>();
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddInfrastructure(_configuration);

        services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        services.AddRouting(options =>
        {
            options.LowercaseUrls = true;
        });

        services.AddMediatR(typeof(Startup));

        services.AddCors(options =>
        {
            options.AddPolicy("TibiaEnemyOtherCharacterFinderApi", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        services
            .AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.DefaultApiVersion = new ApiVersion( 1.0 );
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            // Add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        ConfigureOptions(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
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
        });

        app.UseSerilogRequestLogging(configure =>
        {
            configure.MessageTemplate =
                "HTTP {RequestMethod} {RequestPath} ({UserId}) responded {StatusCode} in {Elapsed:0.0000}ms";
        });
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("TibiaEnemyOtherCharacterFinderApi");
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static void ConfigureOptions(IServiceCollection services)
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