using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Providers;

namespace Shared.Seeder.Configuration;

public static class ConfigureSeederOptions
{
    public static void Configure(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();

        services.Configure<ConnectionStringsSection>(options => configuration.GetSection("ConnectionStrings").Bind(options));
        services.Configure<DapperConfigurationSection>(options => configuration.GetSection("Dapper").Bind(options));
    }

    public static void ConfigureOptions(this ContainerBuilder builder)
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json", optional: false, reloadOnChange: true).Build();
        var configDapper = configuration.GetSection("Dapper");

        //builder.Register(ctx => new ConfigurationChangeTokenSource<DapperConfigurationSection>(Options.DefaultName, sfConfigDapper))
        //       .As<IOptionsChangeTokenSource<DapperConfigurationSection>>()
        //       .SingleInstance();

        builder.Register(ctx => new NamedConfigureFromConfigurationOptions<DapperConfigurationSection>(Options.DefaultName, configDapper, _ => { }))
               .As<IConfigureOptions<DapperConfigurationSection>>()
               .SingleInstance();

        var configConnectionStrings = configuration.GetSection("ConnectionStrings");

        //builder.Register(ctx => new ConfigurationChangeTokenSource<ConnectionStringsSection>(Options.DefaultName, sfConfigConnectionStrings))
        //       .As<IOptionsChangeTokenSource<ConnectionStringsSection>>()
        //       .SingleInstance();

        builder.Register(ctx => new NamedConfigureFromConfigurationOptions<ConnectionStringsSection>(Options.DefaultName, configConnectionStrings, _ => { }))
               .As<IConfigureOptions<ConnectionStringsSection>>()
               .SingleInstance();
    }
}