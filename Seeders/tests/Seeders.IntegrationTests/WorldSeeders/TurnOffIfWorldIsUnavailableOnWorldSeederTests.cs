using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.IntegrationTests.WorldSeeders;

[Collection("Seeder test collection")]
public class TurnOffIfWorldIsUnavailableOnWorldSeederTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;
    private readonly Mock<ITibiaDataClient> _tibiaDataClientMock = new();

    public TurnOffIfWorldIsUnavailableOnWorldSeederTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
        
        var worldNames = new List<string>
        {
            "Adra"
        };
        
        _tibiaDataClientMock.Setup(r => r.FetchWorldsNames()).ReturnsAsync(worldNames);
    }
    
    [Fact]
    public async Task WorldSeeder_TurnOffIfWorldIsUnavailable_ShouldSoftDeleteEachUnavailableWorld()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IRepository>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var worldSeeder = new WorldSeeder.WorldSeederService(repository, _tibiaDataClientMock.Object);
        await worldSeeder.SetProperties();
        
        // Act
        await worldSeeder.TurnOffIfWorldIsUnavailable();
        var worlds = dbContext.Worlds.AsNoTracking().ToList();

        // Assert
        worlds.Count.Should().Be(2);            
        worlds.Count(w => w.IsAvailable).Should().Be(1);            
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}