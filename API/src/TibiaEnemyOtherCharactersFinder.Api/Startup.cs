using Autofac;
using MediatR;
using System.Reflection;
using System.Text.Json.Serialization;
using TibiaEnemyOtherCharactersFinder.Infrastructure;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace TibiaEnemyOtherCharactersFinder.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
        LoggerConfiguration.ConfigureLogger(_configuration);
    }

    // This method gets called by the runtime. Use this method to add services to the container.
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

        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Version = "v1",
                Title = "Tibia Enemy Other Characters Finder API",
                Description = "API for retrieving other characters of our enemy"
            });
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            s.IncludeXmlComments(xmlPath);
        });

        ConfigureApplicationOptions.Configure(services);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TibiaEnemyOtherCharactersFinder API v1");
                    c.RoutePrefix = string.Empty;
                });
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("TibiaEnemyOtherCharacterFinderApi");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}