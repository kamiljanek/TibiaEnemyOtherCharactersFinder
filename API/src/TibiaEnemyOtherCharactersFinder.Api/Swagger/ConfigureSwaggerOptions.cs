using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TibiaEnemyOtherCharactersFinder.Api.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) => _provider = provider;

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }
    }

    private static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Tibia Enemy Other Characters Finder API",
            Version = description.ApiVersion.ToString(),
            Description = "API for retrieving other characters of our enemy",
            Contact = new OpenApiContact { Name = "Kamil Janek", Url = new Uri("https://github.com/kamiljanek") },
            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://github.com/kamiljanek/Tibia-EnemyOtherCharactersFinder/blob/develop/LICENSE.md") }
        };

        if (description.IsDeprecated)
        {
            info.Description += " This API version has been deprecated.";
        }

        return info;
    }
}