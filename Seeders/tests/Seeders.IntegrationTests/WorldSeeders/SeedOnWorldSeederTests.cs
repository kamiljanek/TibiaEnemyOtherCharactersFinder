using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.WorldSeeders;

[Collection("Seeder test collection")]
public class SeedOnWorldSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    private readonly Mock<ITibiaDataClient> _tibiaDataClientMock = new();

    public SeedOnWorldSeederTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
        
        var worldNames = new List<string>
        {
            "Adra",
            "Bastia",
            "Celebra",
            "Harmonia",
            "Syrena",
        };
        
        _tibiaDataClientMock.Setup(r => r.FetchWorldsNames()).ReturnsAsync(worldNames);
    }
    
    [Fact]
    public async Task WorldSeeder_Seed_ShouldCreateOnlyNewWorlds()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ITibiaCharacterFinderDbContext>();
        var worldSeeder = new WorldSeeder.WorldSeederService(dbContext, _tibiaDataClientMock.Object);
        await worldSeeder.SetProperties();

        // Act
        await worldSeeder.Seed();
        var dbContextAfter = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var result = dbContextAfter.Worlds.AsNoTracking().ToList();

        // Assert
        result.Count.Should().Be(5);            
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}