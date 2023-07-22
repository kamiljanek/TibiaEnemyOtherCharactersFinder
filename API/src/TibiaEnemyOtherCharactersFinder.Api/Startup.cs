using Autofac;
using MediatR;
using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using Serilog;
using Shared.RabbitMQ;
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

        services.AddSwaggerGen(settings =>
        {
            settings.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Tibia Enemy Other Characters Finder API",
                Description = "API for retrieving other characters of our enemy",
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Url = new Uri("https://github.com/kamiljanek")
                }
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            settings.IncludeXmlComments(xmlPath);
        });

        ConfigureOptions(services);
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // if (env.IsDevelopment())
        // {
        //     app.UseDeveloperExceptionPage();
        // }
        // UNDONE: możliwe że powyższe do wywalenia
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "TibiaEnemyOtherCharactersFinder API v1");
            c.RoutePrefix = string.Empty;
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