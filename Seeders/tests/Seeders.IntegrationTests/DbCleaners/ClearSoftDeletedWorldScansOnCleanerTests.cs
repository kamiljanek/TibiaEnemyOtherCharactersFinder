using DbCleaner;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.IntegrationTests.DbCleaners;

[Collection("Seeder test collection")]
public class ClearSoftDeletedWorldScansOnCleanerTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;

    public ClearSoftDeletedWorldScansOnCleanerTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }
    
    [Fact]
    public async Task Cleaner_ClearSoftDeletedWorldScans_ShouldDeleteOnlySoftDeletedScans()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var cleaner = scope.ServiceProvider.GetRequiredService<Cleaner>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        var toDelete = dbContext.WorldScans.OrderBy(ws => ws.ScanCreateDateTime).Skip(2).ToList();
        toDelete.ForEach(ws => ws.IsDeleted = true);
        await dbContext.SaveChangesAsync();
        
        // Act
        await cleaner.ClearTables();
        await cleaner.VacuumTables();
        var worldScans = dbContext.WorldScans.AsNoTracking().ToList();

        // Assert
        worldScans.Count.Should().Be(2);
    }
    
    public Task InitializeAsync() => Task.CompletedTask;
    
    public Task DisposeAsync() => _resetDatabase();
}