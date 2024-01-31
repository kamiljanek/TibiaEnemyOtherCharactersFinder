using CharacterAnalyser.Managers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.CharacterAnalysers.CharacterActionsSeeder;

[Collection("Seeder test collection")]
public class SeedOnCharacterActionSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;

    public SeedOnCharacterActionSeederTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task CharacterActionSeeder_Seed_SaveCorrectDataInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<CharacterActionsManager>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITibiaCharacterFinderDbContext>();

        var worldScans = await dbContext.WorldScans
            .Where(scan => scan.WorldId == 11 && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .AsNoTracking()
            .ToListAsync();

        seeder.GetAndSetLoginNames(worldScans);
        seeder.GetAndSetLogoutNames(worldScans);
        
        // Act
        await seeder.SeedCharacterActions(worldScans);
        var characterActions = dbContext.CharacterActions;
        var characters = dbContext.Characters;

        // Assert
        characterActions.Count().Should().Be(10);
        characters.Count().Should().Be(4);
        characters.Count(c => c.FoundInScan).Should().Be(3);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}