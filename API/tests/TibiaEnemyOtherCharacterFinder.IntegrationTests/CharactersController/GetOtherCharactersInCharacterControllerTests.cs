using System.Net;
using FluentAssertions;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;

public class GetOtherCharactersInCharacterControllerTests : IClassFixture<TibiaApiFactory>
{
    private const string _controllerBase = "api/character";
    private const string _defaultName = "_defaultName";
    private const string _nameInDatabase = "duzzerah";
    private readonly TibiaApiFactory _factory;

    public GetOtherCharactersInCharacterControllerTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetOtherCharactersEndpoint_WithRouteParameters_ReturnsStatusOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{_controllerBase}/{_defaultName}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOtherCharactersEndpoint_WithRouteParametersThatFoundInDatabase_ReturnsCorrectData()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{_controllerBase}/{_nameInDatabase}");
        var result = await response.Content.ReadFromJsonAsync<List<CharacterWithCorrelationsResult>>();

        // Assert
        result.Should().NotBeEmpty();
        result!.Count.Should().Be(2);
        result.First(c => c.OtherCharacterName == "abargo maewa").NumberOfMatches.Should().Be(4);
    }
    
    [Fact]
    public async Task GetOtherCharactersEndpoint_WithRouteParametersThatNotFoundInDatabase_ReturnsEmptyList()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{_controllerBase}/{_defaultName}");
        var result = await response.Content.ReadFromJsonAsync<List<CharacterWithCorrelationsResult>>();

        // Assert
        result.Should().BeEmpty();
        result.Should().BeEmpty();
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