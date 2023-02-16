using CharacterAnalyser.Modules;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

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
        var seeder = scope.ServiceProvider.GetRequiredService<CharacterActionSeeder>();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var worldScans = await repository.GetFirstTwoWorldScansAsync(worldId: 11);
        seeder.GetLoginNames(worldScans);
        seeder.GetLogoutNames(worldScans);
        
        // Act
        await seeder.Seed(worldScans);
        var characterActions = dbContext.CharacterActions.ToList();

        // Assert
        characterActions.Count.Should().Be(10);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}