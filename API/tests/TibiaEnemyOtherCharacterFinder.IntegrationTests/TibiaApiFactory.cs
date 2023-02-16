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
                Password = "password"
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
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TibiaCharacterFinderDbContext>));
            if (descriptor != null) 
                services.Remove(descriptor);
            
            services.AddSingleton(Options.Create(new ConnectionStringsSection { PostgreSql = _dbContainer.ConnectionString }));
            services.AddDbContext<TibiaCharacterFinderDbContext>(options => options.UseNpgsql(_dbContainer.ConnectionString).UseSnakeCaseNamingConvention());
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
            new() { WorldId = 111, Name = "Premia", Url = "urlPremia" },
            new() { WorldId = 112, Name = "Vunira", Url = "urlVunira" }
        };
    }
    
    private IEnumerable<Character> CreateCharacters()
    {
        return new List<Character>()
        {
            new() { CharacterId = 111, Name = "abargo maewa", WorldId = 111 },
            new() { CharacterId = 112, Name = "amy winehousse", WorldId = 111 },
            new() { CharacterId = 113, Name = "duzzerah", WorldId = 111 },
    
            new() { CharacterId = 114, Name = "ziomal rafiego", WorldId = 112},
            new() { CharacterId = 115, Name = "zanteey", WorldId = 112 },
            new() { CharacterId = 116, Name = "artenian", WorldId = 112 },
        };
    }
    
    private IEnumerable<CharacterCorrelation> CreateCharacterCorrelaions()
    {
        return new List<CharacterCorrelation>()
        {
            new() { LoginCharacterId = 111, LogoutCharacterId = 112, NumberOfMatches = 8 },
            new() { LoginCharacterId = 113, LogoutCharacterId = 111, NumberOfMatches = 4 },
            new() { LoginCharacterId = 113, LogoutCharacterId = 112, NumberOfMatches = 2 },
    
            new() { LoginCharacterId = 114, LogoutCharacterId = 116, NumberOfMatches = 10 },
            new() { LoginCharacterId = 116, LogoutCharacterId = 115, NumberOfMatches = 21 },
            new() { LoginCharacterId = 115, LogoutCharacterId = 116, NumberOfMatches = 1 },
        };
    }
}