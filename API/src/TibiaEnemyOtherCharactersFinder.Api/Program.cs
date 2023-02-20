using Autofac.Extensions.DependencyInjection;

namespace TibiaEnemyOtherCharactersFinder.Api;

public class Program
{
    public static Task Main(string[] args)
     => CreateHostBuilder(args).Build().RunAsync();

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    // UNDONE:  dodać serilog
    //.UseSerilog();
}