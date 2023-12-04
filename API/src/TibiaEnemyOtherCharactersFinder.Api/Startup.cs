using Autofac;
using MediatR;
using System.Reflection;
using System.Text.Json.Serialization;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using TibiaEnemyOtherCharactersFinder.Api.Configurations;
using TibiaEnemyOtherCharactersFinder.Api.Filters;
using TibiaEnemyOtherCharactersFinder.Api.Swagger;
using TibiaEnemyOtherCharactersFinder.Infrastructure;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Middlewares;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services.BackgroundServices;

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

        services.AddControllers(opt => { opt.Filters.Add<ErrorHandlingFilterAttribute>(); })
            .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

        services.AddRateLimiter(StartupConfigurations.RateLimitedConfiguration);
        services.AddRouting(options => { options.LowercaseUrls = true; });
        services.AddMediatR(typeof(Startup));
        services.AddSignalR();

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Name = "sessionId";
            options.Cookie.HttpOnly = true;
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.IsEssential = true;
        });

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
                options.DefaultApiVersion = new ApiVersion(1.0);
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

        services.AddTransient<GlobalExceptionHandlingMiddleware>();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddSwaggerGen(options =>
        {
            // Add a custom operation filter which sets default values
            options.OperationFilter<SwaggerDefaultValues>();

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });

        StartupConfigurations.ConfigureOptions(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI(StartupConfigurations.SwaggerUiConfiguration(app));


        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
        app.UseSession();
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None,
            Secure = CookieSecurePolicy.None
        });
        app.UseMiddleware<RequestLoggerMiddleware>();
        app.UseRateLimiter();
        app.UseSerilogRequestLogging(StartupConfigurations.SerilogRequestLoggingConfiguration);
        app.UseCors("TibiaEnemyOtherCharacterFinderApi");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        app.UseSignalrHubs();
    }


}