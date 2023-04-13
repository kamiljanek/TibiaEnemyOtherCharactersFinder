using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.WorldSeeders;

[Collection("Seeder test collection")]
public class SeedOnWorldSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    private readonly Mock<ITibiaDataService> _tibiaApiMock = new();

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
        
        _tibiaApiMock.Setup(r => r.FetchWorldsNames()).ReturnsAsync(worldNames);
    }
    
    [Fact]
    public async Task WorldSeeder_Seed_ShouldCreateOnlyNewWorlds()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var worldSeeder = new WorldSeeder.WorldSeederService(repository, _tibiaApiMock.Object);
        await worldSeeder.SetProperties();
        
        // Act
        await worldSeeder.Seed();
        var result = dbContext.Worlds.AsNoTracking().ToList();

        // Assert
        result.Count.Should().Be(5);            
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}