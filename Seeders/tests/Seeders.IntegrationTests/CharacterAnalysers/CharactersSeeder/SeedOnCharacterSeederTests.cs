using CharacterAnalyser.Modules;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.CharacterAnalysers.CharactersSeeder;

[Collection("Seeder test collection")]
public class SeedOnCharacterSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    public SeedOnCharacterSeederTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task CharacterSeeder_Seed_ShouldAddOnlyUniqueCharactersToDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<CharacterSeederService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        var characterActions =  new List<CharacterAction>
        {
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "aphov", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "asiier", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "belzebubba", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "braws", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "armystrong", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "brytiaggo", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "ergren", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "corvinusik", LogoutOrLoginDate = new DateOnly(2022,11,30)}
        };

        dbContext.CharacterActions.AddRange(characterActions);
        await dbContext.SaveChangesAsync();
        
        // Act
        await seeder.Seed();
        var result = dbContext.Characters.ToList();

        // Assert
        result.Count.Should().Be(8);
    }
    
    [Fact]
    public async Task CharacterSeeder_Seed_EveryCharacterNameAndIdShouldBeUnique()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<CharacterSeederService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        var characterActions =  new List<CharacterAction>
        {
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "aphov", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "asiier", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "belzebubba", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "braws", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "armystrong", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "brytiaggo", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "ergren", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "corvinusik", LogoutOrLoginDate = new DateOnly(2022,11,30)}
        };

        dbContext.CharacterActions.AddRange(characterActions);
        await dbContext.SaveChangesAsync();
        
        // Act
        await seeder.Seed();
        var distinctIds = dbContext.Characters.Select(c => c.CharacterId).Distinct().ToList();
        var distinctNames = dbContext.Characters.Select(c => c.Name).Distinct().ToList();
        var allRecords = dbContext.Characters.ToList();

        // Assert
        Assert.Equal(distinctIds.Count, allRecords.Count);
        Assert.Equal(distinctNames.Count, allRecords.Count);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}