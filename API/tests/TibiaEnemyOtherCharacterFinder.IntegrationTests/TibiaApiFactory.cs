using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Npgsql;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests;

public class TibiaApiFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    public TibiaCharacterFinderDbContext DbContext { get; set; }

    private readonly PostgreSqlTestcontainer _dbContainer =
        new TestcontainersBuilder<PostgreSqlTestcontainer>()
             .WithDatabase(new PostgreSqlTestcontainerConfiguration
             {
                 Database = "postgres",
                 Username = "username",
                 Password = "password",
             })

            .WithImage("postgres:14.6")
            .Build();

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        var insertWorldsCommand = @"INSERT INTO public.worlds (world_id, ""name"", url, is_available) VALUES (1, 'premia', 'urlpremia', true)";
        var insertCharactersCommand = @"INSERT INTO public.""characters"" (character_id , ""name"" , world_id) VALUES (1, 'duzzerah', 1), (2, 'someName', 1)";
        var insertCorrelationsCommand = @"INSERT INTO public.character_correlations (correlation_id, logout_character_id, login_character_id, number_of_matches) VALUES (1, 1, 2, 5)";
        using (var connection = new NpgsqlConnection(_dbContainer.ConnectionString))
        {
            var scriptWorldsCommand = new NpgsqlCommand(insertWorldsCommand, connection);
            var scriptCharactersCommand = new NpgsqlCommand(insertCharactersCommand, connection);
            var scriptCorrelationsCommand = new NpgsqlCommand(insertCorrelationsCommand, connection);
            await connection.OpenAsync();
            scriptWorldsCommand.ExecuteNonQuery();
            scriptCharactersCommand.ExecuteNonQuery();
            scriptCorrelationsCommand.ExecuteNonQuery();
            await connection.CloseAsync();
        };

        //var optionsBuilder = new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>();
        //optionsBuilder.UseNpgsql(_dbContainer.ConnectionString);
        //var dbContextEFCore = new TibiaCharacterFinderDbContext(optionsBuilder.Options);
        //await dbContext.Database.MigrateAsync();
        //await dbContext.Worlds.AddAsync(new() { WorldId = 2, Name = "Vunira", Url = "urlVunira", IsAvailable = true });
        //var entries = dbContext.ChangeTracker.Entries();
        //await dbContext.SaveChangesAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TibiaCharacterFinderDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            services.RemoveAll(typeof(DbContextOptions<TibiaCharacterFinderDbContext>));
            //services.RemoveAll(typeof(ITibiaCharacterFinderDbContext));
            services.RemoveAll(typeof(TibiaCharacterFinderDbContext));
            services.AddSingleton(Options.Create(new ConnectionStringsSection { PostgreSql = _dbContainer.ConnectionString }));
            //services.AddSingleton<DbContextOptions<TibiaCharacterFinderDbContext>>();
            //services.AddSingleton<TibiaCharacterFinderDbContext>(_ =>
            //{
            var optionsBuilder = new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>();
            optionsBuilder.UseNpgsql(_dbContainer.ConnectionString);
            services.AddSingleton(new TibiaCharacterFinderDbContext(optionsBuilder.Options));
            //});
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            DbContext = scopedServices.GetRequiredService<TibiaCharacterFinderDbContext>();
            services.AddDbContextPool<TibiaCharacterFinderDbContext>(options => options.UseNpgsql(_dbContainer.ConnectionString));
        });

        //builder.ConfigureTestServices(services =>
        //{
        //    // Remove AppDbContext

        //    var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IOptions<ConnectionStringsSection>));
        //    if (descriptor != null) services.Remove(descriptor);

        //    // Add DB context pointing to test container
        //    services.AddSingleton(Options.Create(new ConnectionStringsSection { PostgreSql = _dbContainer.ConnectionString }));
        //    services.AddDbContext<TibiaCharacterFinderDbContext>(options => { options.UseNpgsql(_dbContainer.ConnectionString); });
        //    // Ensure schema gets created
        //    var serviceProvider = services.BuildServiceProvider();
        //    using var scope = serviceProvider.CreateScope();
        //    var scopedServices = scope.ServiceProvider;
        //    var dbContext = scopedServices.GetRequiredService<TibiaCharacterFinderDbContext>();

        //    //context.Database.EnsureCreated();
        //});
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }

    public TibiaCharacterFinderDbContext CreateTibiaDbContext()
    {
        return Services.GetService<TibiaCharacterFinderDbContext>()!;
    }
}