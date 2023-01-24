using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureApplicationOptions
{
    public static void Configure(IServiceCollection services)
    {
        services.Configure<ConnectionStringsSection>(options => GetSettingsSection(ApplicationSettingsSections.ConnectionStringsSection).Bind(options));
        services.Configure<DapperConfigurationSection>(options => GetSettingsSection(ApplicationSettingsSections.DapperSection).Bind(options));
    }

    public static void ConfigureOptions(this ContainerBuilder builder)
    {
        var configDapper = GetSettingsSection(ApplicationSettingsSections.DapperSection);
        var configConnectionStrings = GetSettingsSection(ApplicationSettingsSections.ConnectionStringsSection);

        builder.Register(c => new NamedConfigureFromConfigurationOptions<DapperConfigurationSection>(Options.DefaultName, configDapper, _ => { }))
               .As<IConfigureOptions<DapperConfigurationSection>>()
               .SingleInstance();

        builder.Register(c => new NamedConfigureFromConfigurationOptions<ConnectionStringsSection>(Options.DefaultName, configConnectionStrings, _ => { }))
               .As<IConfigureOptions<ConnectionStringsSection>>()
               .SingleInstance();
    }

    private static IConfigurationSection GetSettingsSection(string settingsSection)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile(ApplicationSettingsSections.FileName, optional: false, reloadOnChange: true).Build();
        var result = configuration.GetSection(settingsSection);

        return result;
    }
}