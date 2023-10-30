using CharacterAnalyser.Modules;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.CharacterAnalysers.CharacterCorrelationsDeleter;

[Collection("Seeder test collection")]
public class DeleteOnCharacterCorrelationDeleterTests : IAsyncLifetime
{
     private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    public DeleteOnCharacterCorrelationDeleterTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task CharacterCorrelationDeleter_Delete_ShouldDeleteRecordsTahtCorrelationExistInOneScan()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var deleter = scope.ServiceProvider.GetRequiredService<CharacterCorrelationDeleter>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE character_correlations CASCADE");
        await dbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE characters CASCADE");
        await SeedDatabaseAsync(dbContext);

        // Act
        var characterCorrelationsBeforeDelete = dbContext.CharacterCorrelations.ToList();
        await deleter.Delete();
        var characterCorrelationsAfterDelete = dbContext.CharacterCorrelations.ToList();
        
        // Assert
        characterCorrelationsBeforeDelete.Count.Should().Be(22);
        characterCorrelationsAfterDelete.Count.Should().Be(17);
    }

    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
    
    private async Task SeedDatabaseAsync(TibiaCharacterFinderDbContext dbContext)
    {
        await dbContext.CharacterActions.AddRangeAsync(GetCharacterActions());
        await dbContext.Characters.AddRangeAsync(GetCharacters());
        await dbContext.CharacterCorrelations.AddRangeAsync(GetCharacterCorrelations());
        
        await dbContext.SaveChangesAsync();
    }
    
    private IEnumerable<CharacterAction> GetCharacterActions()
    {
        return new List<CharacterAction>
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
    }
    
    private IEnumerable<Character> GetCharacters()
    {
        return new List<Character>
        {
            new() {CharacterId = 120, WorldId = 11, Name = "aphov", FoundInScan = true},
            new() {CharacterId = 121, WorldId = 11, Name = "asiier", FoundInScan = true},
            new() {CharacterId = 122, WorldId = 11, Name = "belzebubba"},
            new() {CharacterId = 123, WorldId = 11, Name = "braws"},
            new() {CharacterId = 124, WorldId = 11, Name = "armystrong"},
            new() {CharacterId = 125, WorldId = 11, Name = "brytiaggo", FoundInScan = true},
            new() {CharacterId = 126, WorldId = 11, Name = "ergren"},
            new() {CharacterId = 127, WorldId = 11, Name = "corvinusik", FoundInScan = true},
        };
    }
    
    private IEnumerable<CharacterCorrelation> GetCharacterCorrelations()
    {
        return new List<CharacterCorrelation>
        {
            new() { LoginCharacterId = 120, LogoutCharacterId = 124, NumberOfMatches = 1},
            new() { LoginCharacterId = 120, LogoutCharacterId = 125, NumberOfMatches = 2},
            new() { LoginCharacterId = 120, LogoutCharacterId = 126, NumberOfMatches = 3},
            new() { LoginCharacterId = 120, LogoutCharacterId = 127, NumberOfMatches = 4},
            new() { LoginCharacterId = 121, LogoutCharacterId = 124, NumberOfMatches = 5},
            new() { LoginCharacterId = 121, LogoutCharacterId = 125, NumberOfMatches = 6},
            new() { LoginCharacterId = 121, LogoutCharacterId = 126, NumberOfMatches = 7},
            new() { LoginCharacterId = 121, LogoutCharacterId = 127, NumberOfMatches = 8},
            new() { LoginCharacterId = 122, LogoutCharacterId = 124, NumberOfMatches = 9},
            new() { LoginCharacterId = 122, LogoutCharacterId = 125, NumberOfMatches = 10},
            new() { LoginCharacterId = 122, LogoutCharacterId = 126, NumberOfMatches = 11},
            new() { LoginCharacterId = 122, LogoutCharacterId = 127, NumberOfMatches = 12},
            new() { LoginCharacterId = 123, LogoutCharacterId = 124, NumberOfMatches = 13},
            new() { LoginCharacterId = 123, LogoutCharacterId = 125, NumberOfMatches = 14},
            new() { LoginCharacterId = 123, LogoutCharacterId = 126, NumberOfMatches = 15},
            new() { LoginCharacterId = 123, LogoutCharacterId = 127, NumberOfMatches = 16},
            
            new() { LoginCharacterId = 120, LogoutCharacterId = 123, NumberOfMatches = 17},
            new() { LoginCharacterId = 122, LogoutCharacterId = 121, NumberOfMatches = 18},
            new() { LoginCharacterId = 122, LogoutCharacterId = 123, NumberOfMatches = 19},
            new() { LoginCharacterId = 127, LogoutCharacterId = 124, NumberOfMatches = 20},
            new() { LoginCharacterId = 125, LogoutCharacterId = 126, NumberOfMatches = 21},
            new() { LoginCharacterId = 125, LogoutCharacterId = 127, NumberOfMatches = 22},
        };
    }
}