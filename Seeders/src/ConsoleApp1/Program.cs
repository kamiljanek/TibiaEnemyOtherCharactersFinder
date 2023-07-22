using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
// using ChangeNameDetector.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Configuration;

namespace ConsoleApp1;




public class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var host = CreateHostBuilder(args);

            Log.Information("Starting application");

            // var service = ActivatorUtilities.CreateInstance<ChangeNameDetectorService>(host.Services);
            // await service.Run();

            Log.Information("Ending application properly");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static IHost CreateHostBuilder(string [] args)
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    // .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    // .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                builder.RegisterModule<AutofacModule>();
            })
            .ConfigureServices((context, services) =>
            {
                services
                    // .AddNameDetector()
                    .AddServices()
                    .AddSerilog(context.Configuration, Assembly.GetExecutingAssembly().GetName().Name)
                    .AddTibiaDbContext(context.Configuration);
            })
            .UseSerilog()
            .Build();

        return host;
        // UNDONE: wyeksportowac ta metode do shared
    }
}

// 1. dorobienie kolumny do tabeli characters z datą sprawdzania postaci w poszukiwaniu zmiany name
// 2. skanowanie każdej postaci i sprawdzanie czy wpisany name znajduje się w "former names" (rabbit czy bez?)(pool requesty czy nie?)
// 3. sprawdzam czy znalazł character po name
//      a) jeżeli json jest pusty to znaczy że nie znalazł i wywal taką postać całkowicie z bazy
//      b) jeżeli timeOut to powtórz zapytanie
// 4. jeżeli znalazł zmiane name (czyli szukany name jest w propercie "former names" dodaj (muszę przemyśleć ile rekordów ma przeszukać) matching_number
//      to nowego name (po prostu je zsumuj na nowym name) i wykasuj stary name kaskadowo
// 5. Zmienić Character Get endpoint tak aby przeszukiwał w bazie tylko po nowym name a nie po wpisanym przez uzytkownika
// 6. Podczas szukania można odrazu sprawdzać czy postać jest traded i wtedy kasować wszystkie correlations powiązane z tą postacią
// UNDONE: