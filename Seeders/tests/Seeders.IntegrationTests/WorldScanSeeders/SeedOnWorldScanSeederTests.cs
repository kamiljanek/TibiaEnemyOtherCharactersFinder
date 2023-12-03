using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;
using WorldScanSeeder;

namespace Seeders.IntegrationTests.WorldScanSeeders;

[Collection("Seeder test collection")]
public class SeedOnWorldScanSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    private readonly Mock<ITibiaDataClient> _tibiaDataClientMock = new();

    public SeedOnWorldScanSeederTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task WorldScanSeeder_Seed_ShouldCreateNewRecordInDatabase()
    {
        // Arrange
        var listOfNames = new List<string>() { "aphov", "armystrong", "asiier", "braws" };
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var worlds = dbContext.Worlds.AsNoTracking().ToList();
        _tibiaDataClientMock.Setup(r => r.FetchCharactersOnline(worlds[0].Name)).ReturnsAsync(listOfNames);
        var worldScanSeeder = new ScanSeeder(repository, _tibiaDataClientMock.Object);
        await worldScanSeeder.SetProperties();
        
        // Act
        var resultBeforeSeed = dbContext.WorldScans.AsNoTracking().ToList();
        await worldScanSeeder.Seed(worlds[0]);
        var resultAfterSeed = dbContext.WorldScans.AsNoTracking().ToList();

        // Assert
        (resultAfterSeed.Count-resultBeforeSeed.Count).Should().Be(1);            
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}