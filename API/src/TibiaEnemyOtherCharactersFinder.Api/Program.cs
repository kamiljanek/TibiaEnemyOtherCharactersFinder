using Autofac.Extensions.DependencyInjection;
using Serilog;

namespace TibiaEnemyOtherCharactersFinder.Api;

public class Program
{
    public static Task Main(string[] args)
    {
        return CreateHostBuilder(args).Build().RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .UseSerilog();
    }
}