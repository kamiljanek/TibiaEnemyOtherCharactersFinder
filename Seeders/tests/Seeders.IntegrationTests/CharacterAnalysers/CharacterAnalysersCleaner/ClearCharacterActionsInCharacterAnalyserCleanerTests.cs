using CharacterAnalyser.Modules;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.IntegrationTests.CharacterAnalysers.CharacterAnalysersCleaner;

[Collection("Seeder test collection")]
public class ClearCharacterActionsInCharacterAnalyserCleanerTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;

    public ClearCharacterActionsInCharacterAnalyserCleanerTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task CharacterAnalyserCleaner_ClearCharacterActions_ShouldDeleteAllRecordsFromCharacterAnalysersTable()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var cleaner = scope.ServiceProvider.GetRequiredService<CharacterActionsCleaner>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        
        var characterActions =  new List<CharacterAction>
        {
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3217, CharacterName = "aphov", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = true, WorldScanId = 3254, CharacterName = "asiier", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3278, CharacterName = "armystrong", LogoutOrLoginDate = new DateOnly(2022,11,30)},
            new() { WorldId = 11, IsOnline = false, WorldScanId = 3298, CharacterName = "braws", LogoutOrLoginDate = new DateOnly(2022,11,30)},
        };
        dbContext.CharacterActions.AddRange(characterActions);
        
        // Act
        await cleaner.ClearAsync();
        var result = dbContext.CharacterActions.ToList();

        // Assert
        result.Should().BeEmpty();            
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}