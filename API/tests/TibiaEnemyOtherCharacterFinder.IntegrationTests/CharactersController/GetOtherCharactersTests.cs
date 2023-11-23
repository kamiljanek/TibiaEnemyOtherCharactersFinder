using System.Net;
using FluentAssertions;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;

public class GetOtherCharactersTests : CharactersControllerTestTemplate, IClassFixture<TibiaApiFactory>
{
    private const string ControllerBase = "api/tibia-eocf/v1/characters";
    private const string DefaultName = "Default Name";
    private const string NameInDatabase = "duzzerah";
    private readonly TibiaApiFactory _factory;

    public GetOtherCharactersTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetOtherCharactersEndpoint_WithRouteParametersThatFoundInDatabase_ShouldReturnsCorrectData()
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
    public async Task GetOtherCharactersEndpoint_WithRouteParametersThatNotFoundInDatabase_ShouldReturnStatusNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.GetAsync($"{ControllerBase}/{DefaultName}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [MemberData(nameof(GetInvalidLengthRouteParameters))]
    public async Task GetOtherCharactersEndpoint_WithInvalidLenghtRouteParameters_ShouldReturnsStatusBadRequest(string parameter)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.GetAsync($"{ControllerBase}/{parameter}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(GetUnacceptableRouteParameters))]
    public async Task GetOtherCharactersEndpoint_WithUnacceptableCharactersInRouteParameters_ShouldReturnsStatusBadRequest(string parameter)
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.GetAsync($"{ControllerBase}/{parameter}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}