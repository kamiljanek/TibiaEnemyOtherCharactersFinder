using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TibiaEnemyOtherCharactersFinder.Api;
using TibiaEnemyOtherCharactersFinder.Application.Configuration.Settings;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
#pragma warning disable CS0618

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests;

public class TibiaApiFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
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
        await dbContext.Database.MigrateAsync();
        
        await dbContext.Worlds.AddRangeAsync(CreateWorlds());
        await dbContext.Characters.AddRangeAsync(CreateCharacters());
        await dbContext.CharacterCorrelations.AddRangeAsync(CreateCharacterCorrelaions());
        
        await dbContext.SaveChangesAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TibiaCharacterFinderDbContext>));
            if (descriptor != null) services.Remove(descriptor);
            services.AddSingleton(Options.Create(new ConnectionStringsSection
                { PostgreSql = _dbContainer.ConnectionString }));
            services.AddDbContext<TibiaCharacterFinderDbContext>(options =>
                options.UseNpgsql(_dbContainer.ConnectionString).UseSnakeCaseNamingConvention());
        });
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.DisposeAsync();
    }
    
    private IEnumerable<World> CreateWorlds()
    {
        return new List<World>()
        {
            new() { WorldId = 1, Name = "Premia", Url = "urlPremia"},
            new() { WorldId = 2, Name = "Vunira", Url = "urlVunira"}
        };
    }
    
    private IEnumerable<Character> CreateCharacters()
    {
        return new List<Character>()
        {
            new() { CharacterId = 1, Name = "abargo maewa", WorldId = 1 },
            new() { CharacterId = 2, Name = "amy winehousse", WorldId = 1 },
            new() { CharacterId = 3, Name = "duzzerah", WorldId = 1 },
    
            new() { CharacterId = 4, Name = "ziomal rafiego", WorldId = 2},
            new() { CharacterId = 5, Name = "zanteey", WorldId = 2 },
            new() { CharacterId = 6, Name = "artenian", WorldId = 2 },
        };
    }
    
    private IEnumerable<CharacterCorrelation> CreateCharacterCorrelaions()
    {
        return new List<CharacterCorrelation>()
        {
            new() { LoginCharacterId = 1, LogoutCharacterId = 2, NumberOfMatches = 8 },
            new() { LoginCharacterId = 3, LogoutCharacterId = 1, NumberOfMatches = 4 },
            new() { LoginCharacterId = 3, LogoutCharacterId = 2, NumberOfMatches = 2 },
    
            new() { LoginCharacterId = 4, LogoutCharacterId = 6, NumberOfMatches = 10 },
            new() { LoginCharacterId = 6, LogoutCharacterId = 5, NumberOfMatches = 21 },
            new() { LoginCharacterId = 5, LogoutCharacterId = 6, NumberOfMatches = 1 },
        };
    }
}