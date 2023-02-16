using CharacterAnalyser.Modules;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.IntegrationTests.CharacterAnalysers.CharacterCorrelationsSeeder;

[Collection("Seeder test collection")]
public class SeedOnCharacterCorrelationSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    public SeedOnCharacterCorrelationSeederTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task CharacterCorrelationSeeder_Seed_ShouldUpdateNumberOfMatchWhenRecordExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var correlationSeeder = scope.ServiceProvider.GetRequiredService<CharacterCorrelationSeeder>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        var characterActions =  new List<CharacterAction>
        {
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "aphov", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "asiier", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "armystrong", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "brytiaggo", LogoutOrLoginDate = new DateOnly(2022,11,30)},
        };
        
        dbContext.CharacterActions.AddRange(characterActions);
        await dbContext.SaveChangesAsync();
        
        // Act
        await correlationSeeder.Seed();
        var result = dbContext.CharacterCorrelations.AsNoTracking().ToList();

        // Assert
        result.First(c => c is { LoginCharacterId: 120, LogoutCharacterId: 123 }).NumberOfMatches.Should().Be(7);
        result.First(c => c is { LoginCharacterId: 120, LogoutCharacterId: 122 }).NumberOfMatches.Should().Be(4);
    }
    
    [Fact]
    public async Task CharacterCorrelationSeeder_Seed_ShouldCreateNewIfRecordNotExist()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var characterSeeder = scope.ServiceProvider.GetRequiredService<CharacterSeeder>();
        var correlationSeeder = scope.ServiceProvider.GetRequiredService<CharacterCorrelationSeeder>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        var characterActions =  new List<CharacterAction>
        {
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "aphov", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "asiier", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "braws", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "armystrong", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3217, CharacterName = "brytiaggo", LogoutOrLoginDate = new DateOnly(2022,11,30)},
        };
        
        dbContext.CharacterActions.AddRange(characterActions);
        await dbContext.SaveChangesAsync();
        await characterSeeder.Seed();
        
        // Act
        await correlationSeeder.Seed();
        var result = dbContext.CharacterCorrelations.AsNoTracking().ToList();

        // Assert
        result.Count.Should().Be(6);
        
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}