using CharacterAnalyser;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.IntegrationTests.CharacterAnalysers;

[Collection("Seeder test collection")]
public class AnalyserFullTests : IAsyncLifetime
{
    private readonly TibiaSeederFactory _factory;
    private readonly Func<Task> _resetDatabase;

    public AnalyserFullTests(TibiaSeederFactory factory)
    {
        _factory = factory;
        _resetDatabase = factory.ResetDatabaseAsync;
    }

    [Fact]
    public async Task WorldScanSeeder_Seed_ShouldCreateNewRecordInDatabase()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var analyser = scope.ServiceProvider.GetRequiredService<Analyser>();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();
        
        await _factory.ClearDatabaseAsync(dbContext);
        await SeedDatabseAsync(dbContext);
        
        // Act
        while (await analyser.HasDataToAnalyse())
        {
            foreach (var worldId in analyser.UniqueWorldIds)
            {
                var worldScans = await analyser.GetWorldScansToAnalyseAsync(worldId);
                await analyser.Seed(worldScans);
            }
        }

        // Assert
        var scans = dbContext.WorldScans.AsNoTracking().ToList();
        var actions = dbContext.CharacterActions.AsNoTracking().ToList();
        var characters = dbContext.Characters.AsNoTracking().ToList();
        var correlations = dbContext.CharacterCorrelations.AsNoTracking().ToList();

        scans.Count.Should().Be(12);
        scans.Count(s => !s.IsDeleted).Should().Be(2);
        scans.Count(s => s.IsDeleted).Should().Be(10);
        
        actions.Count.Should().Be(4);
        actions.Count(a => !a.IsOnline).Should().Be(3);
        actions.Count(a => a.IsOnline).Should().Be(1);
        actions.All(s => s.WorldId == 31).Should().Be(true);

        characters.Count.Should().Be(16);
        characters.Select(c => c.Name).Distinct().Count().Should().Be(characters.Count);
        characters.Count(c => c.WorldId == 31).Should().Be(11);
        characters.Count(c => c.WorldId == 32).Should().Be(5);

        correlations.Count.Should().Be(27);
        correlations.Select(c => (c.LogoutCharacterId, c.LoginCharacterId)).Distinct().Count().Should().Be(correlations.Count);
        correlations.Count(c => c.NumberOfMatches == 1).Should().Be(25);
        correlations.Count(c => c.NumberOfMatches == 2).Should().Be(2);
        
        //check distinct values but switches columns
        var testList1 = correlations.Select(c => new Tuple<int, int>(c.LogoutCharacterId, c.LoginCharacterId));
        var testList2 = correlations.Select(c => new Tuple<int, int>(c.LoginCharacterId, c.LogoutCharacterId));
        testList1.Intersect(testList2).Should().BeEmpty();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return _resetDatabase();
    }
    
    private async Task SeedDatabseAsync(TibiaCharacterFinderDbContext dbContext)
    {
        await dbContext.Worlds.AddRangeAsync(GetWorlds());
        await dbContext.WorldScans.AddRangeAsync(GetWorldScans());
        
        await dbContext.SaveChangesAsync();
    }
    
    private IEnumerable<World> GetWorlds()
    {
        return new List<World>
        {
            new() { WorldId = 31, Name = "Damora", IsAvailable = true, Url = "https://www.tibia.com/community/?subtopic=worlds&world=Damora" },
            new() { WorldId = 32, Name = "Epoca", IsAvailable = true, Url = "https://www.tibia.com/community/?subtopic=worlds&world=Epoca" },
            new() { WorldId = 33, Name = "Harmonia", IsAvailable = false, Url = "https://www.tibia.com/community/?subtopic=worlds&world=Harmonia" } // do przemyślenia czy to porzebne
        };
    }

    private IEnumerable<WorldScan> GetWorldScans()
    {
        return new List<WorldScan>
        {
            new() { WorldScanId = 3182, IsDeleted = true, WorldId = 31, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aaa|bbb|hhh|lll" },
            new() { WorldScanId = 3217, IsDeleted = false, WorldId = 31, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aaa|bbb|ccc|fff|ggg|iii" },// cfi - djkl
            new() { WorldScanId = 3302, IsDeleted = false, WorldId = 31, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "aaa|bbb|ddd|ggg|jjj|kkk|lll" },// bgjk - ch
            new() { WorldScanId = 3387, IsDeleted = false, WorldId = 31, ScanCreateDateTime = new DateTime(2022,11,30,20,33,52, DateTimeKind.Utc), CharactersOnline = "aaa|ccc|ddd|hhh|lll" },// dhl - e
            new() { WorldScanId = 3472, IsDeleted = false, WorldId = 31, ScanCreateDateTime = new DateTime(2022,11,30,20,39,06, DateTimeKind.Utc), CharactersOnline = "aaa|ccc|eee" },
            
            new() { WorldScanId = 3727, IsDeleted = false, WorldId = 31, ScanCreateDateTime = new DateTime(2022,11,30,20,55,02, DateTimeKind.Utc), CharactersOnline = "aaa|bbb|ccc|ddd|fff|ggg" },
            
            
            new() { WorldScanId = 3183, IsDeleted = true, WorldId = 32, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "222|333|444" },
            new() { WorldScanId = 3218, IsDeleted = false, WorldId = 32, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "111" },
            new() { WorldScanId = 3303, IsDeleted = false, WorldId = 32, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "111|333|555|666|999" },// 569 - 47
            new() { WorldScanId = 3388, IsDeleted = false, WorldId = 32, ScanCreateDateTime = new DateTime(2022,11,30,20,33,52, DateTimeKind.Utc), CharactersOnline = "111|333|444|777" },
            new() { WorldScanId = 3473, IsDeleted = false, WorldId = 32, ScanCreateDateTime = new DateTime(2022,11,30,20,39,06, DateTimeKind.Utc), CharactersOnline = "333" },
                                                  
            new() { WorldScanId = 3813, IsDeleted = false, WorldId = 32, ScanCreateDateTime = new DateTime(2022,11,30,21,00,18, DateTimeKind.Utc), CharactersOnline = "111|222|444|555" }
        };
    }
}