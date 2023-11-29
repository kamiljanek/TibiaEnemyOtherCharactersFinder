using CharacterAnalyser.Managers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
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
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        var worldScans = await repository.GetFirstTwoWorldScansAsync(worldId: 11);
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