using System.Net;
using FluentAssertions;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;

public class GetOtherCharactersInCharacterControllerTests : IClassFixture<TibiaApiFactory>
{
    private const string ControllerBase = "api/character";
    private const string DefaultName = "_defaultName";
    private const string NameInDatabase = "duzzerah";
    private readonly TibiaApiFactory _factory;

    public GetOtherCharactersInCharacterControllerTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetOtherCharactersEndpoint_WithRouteParametersThatFoundInDatabase_ReturnsCorrectData()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{ControllerBase}/{NameInDatabase}");
        var result = await response.Content.ReadFromJsonAsync<CharacterWithCorrelationsResult>();

        // Assert
        result.Should().NotBeNull();
        result!.PossibleInvisibleCharacters.Count.Should().Be(2);
        result.PossibleInvisibleCharacters.First(c => c.OtherCharacterName == "abargo maewa").NumberOfMatches.Should().Be(4);
    }
    
    [Fact]
    public async Task GetOtherCharactersEndpoint_WithRouteParametersThatNotFoundInDatabase_ReturnsEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.GetAsync($"{ControllerBase}/{DefaultName}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public void NumberOfElementsInDatabaseShouldBeCorrect()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        // Act
        var worlds = dbContext.Worlds.ToList();
        var characters = dbContext.Characters.ToList();
        var characterCorrelations = dbContext.CharacterCorrelations.ToList();

        // Assert
        worlds.Count.Should().Be(2);
        characters.Count.Should().Be(6);
        characterCorrelations.Count.Should().Be(6);
    }
}