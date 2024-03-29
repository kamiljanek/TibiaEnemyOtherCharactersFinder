using CharacterAnalyser.Managers;
using FluentAssertions;
using Moq;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.Tests.CharacterAnalysers.CharacterActionSeeders;

public class GetLogoutNamesInCharacterActionSeederTests
{
    private readonly Mock<ITibiaCharacterFinderDbContext> _dxContextMock = new();

    public GetLogoutNamesInCharacterActionSeederTests()
    {
    }
    
    [Fact]
    public void GetLogoutNamesInCharacterActionSeederReturnsExpectedResult()
    {
        // Arrange
        var worldScans = new List<WorldScan>
        {
            new() { WorldScanId = 3217, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|burntmeat|fosani|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador|kiperr the third"},
            new() { WorldScanId = 3302, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|brytiaggo|fresita linda|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador"},
        };
        var characterActionSeeder = new CharacterActionsManager(_dxContextMock.Object);
        
        // Act
        var logoutNames = characterActionSeeder.GetAndSetLogoutNames(worldScans);

        // Assert
        logoutNames.Count.Should().Be(3);
    }
    
    [Fact]
    public void GetLogoutNamesInCharacterActionSeederReturnsEmptyListIfEveryNameFromFirstWorldScanExistInSecondWorldScan()
    {
        // Arrange
        var worldScans = new List<WorldScan>
        {
            new() { WorldScanId = 3217, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador"},
            new() { WorldScanId = 3302, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|brytiaggo|fresita linda|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador"},
        };
        var characterActionSeeder = new CharacterActionsManager(_dxContextMock.Object);
        
        // Act
        var logoutNames = characterActionSeeder.GetAndSetLogoutNames(worldScans);

        // Assert
        logoutNames.Count.Should().Be(0);
    }
}