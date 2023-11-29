using CharacterAnalyser.Managers;
using FluentAssertions;
using Moq;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace Seeders.Tests.CharacterAnalysers.CharacterActionSeeders;

public class GetLoginNamesInCharacterActionSeederTests
{
    private readonly Mock<IRepository> _repositoryMock = new();

    public GetLoginNamesInCharacterActionSeederTests()
    {
    }
    
    [Fact]
    public void GetLoginNamesInCharacterActionSeederReturnsExpectedResult()
    {
        // Arrange
        var worldScans = new List<WorldScan>
        {
            new() { WorldScanId = 3217, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|burntmeat|fosani|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador|kiperr the third"},
            new() { WorldScanId = 3302, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|brytiaggo|fresita linda|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador"},
        };
        var characterActionSeeder = new CharacterActionsManager(_repositoryMock.Object);
        
        // Act
        var loginNames = characterActionSeeder.GetAndSetLoginNames(worldScans);

        // Assert
        loginNames.Count.Should().Be(2);
    }
    
    [Fact]
    public void GetLoginNamesInCharacterActionSeederReturnsEmptyListIfEveryNameFromSecondWorldScanExistInFirstWorldScan()
    {
        // Arrange
        var worldScans = new List<WorldScan>
        {
            new() { WorldScanId = 3217, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,23,12, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|burntmeat|fosani|friedbert|ganancia adra|guga falido|just mojito|kinaduh|kineador|kiperr the third"},
            new() { WorldScanId = 3302, WorldId = 1, ScanCreateDateTime = new DateTime(2022,11,30,20,28,36, DateTimeKind.Utc), CharactersOnline = "aphov|armystrong|asiier|braws|ganancia adra|guga falido|just mojito|kinaduh|kineador"},
        };
        var characterActionSeeder = new CharacterActionsManager(_repositoryMock.Object);
        
        // Act
        var loginNames = characterActionSeeder.GetAndSetLoginNames(worldScans);

        // Assert
        loginNames.Count.Should().Be(0);
    }
}