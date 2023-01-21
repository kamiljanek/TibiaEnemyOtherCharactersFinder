using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;

namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

public static class ConfigureAppOptions
{
    private const string _settingFileName = "appsettings.Development.json";
    private const string _settingDapperSection = "Dapper";
    private const string _settingConnectionStringsSection = "ConnectionStrings";

    public static void Configure(IServiceCollection services)
    {
        services.Configure<ConnectionStringsSection>(options => GetSettingsSection(_settingConnectionStringsSection).Bind(options));
        services.Configure<DapperConfigurationSection>(options => GetSettingsSection(_settingDapperSection).Bind(options));
    }

    public static void ConfigureOptions(this ContainerBuilder builder)
    {
        var configDapper = GetSettingsSection(_settingDapperSection);
        var configConnectionStrings = GetSettingsSection(_settingConnectionStringsSection);

        builder.Register(c => new NamedConfigureFromConfigurationOptions<DapperConfigurationSection>(Options.DefaultName, configDapper, _ => { }))
               .As<IConfigureOptions<DapperConfigurationSection>>()
               .SingleInstance();

        builder.Register(c => new NamedConfigureFromConfigurationOptions<ConnectionStringsSection>(Options.DefaultName, configConnectionStrings, _ => { }))
               .As<IConfigureOptions<ConnectionStringsSection>>()
               .SingleInstance();
    }

    private static IConfigurationSection GetSettingsSection(string settingsSection)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile(_settingFileName, optional: false, reloadOnChange: true).Build();
        var result = configuration.GetSection(settingsSection);

        return result;
    }
}