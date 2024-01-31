using FluentAssertions;
using TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.WorldsController;

public class GetWorldsTests : CharactersControllerTestTemplate, IClassFixture<TibiaApiFactory>
{
    private const string ControllerBase = "api/tibia-eocf/v1/worlds";
    private readonly TibiaApiFactory _factory;

    public GetWorldsTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetWorldsEndpoint_WithTrueInParameter_ShouldReturnsAvailableWorlds()
    {
        // Arrange
        var client = _factory.CreateClient();
        var isAvailable = true;
        var additionalParameters = $"?{nameof(isAvailable)}={isAvailable}";

        // Act
        var response = await client.GetAsync($"{ControllerBase}{additionalParameters}");
        var result = await response.Content.ReadFromJsonAsync<GetWorldsResult>();

        // Assert
        result.Should().NotBeNull();
        result!.Worlds.Count.Should().Be(2);
        result.Worlds[0].Name.Should().Be("Premia");
        result.Worlds[1].Name.Should().Be("Vunira");
    }

    [Fact]
    public async Task GetWorldsEndpoint_WithFalseInParameter_ShouldReturnsUnavailableWorlds()
    {
        // Arrange
        var client = _factory.CreateClient();
        var isAvailable = false;
        var additionalParameters = $"?{nameof(isAvailable)}={isAvailable}";

        // Act
        var response = await client.GetAsync($"{ControllerBase}{additionalParameters}");
        var result = await response.Content.ReadFromJsonAsync<GetWorldsResult>();

        // Assert
        result.Should().NotBeNull();
        result!.Worlds.Count.Should().Be(1);
        result.Worlds[0].Name.Should().Be("Antica");
    }

    [Fact]
    public async Task GetWorldsEndpoint_WithoutParameter_ShouldReturnsAllWorlds()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{ControllerBase}");
        var result = await response.Content.ReadFromJsonAsync<GetWorldsResult>();

        // Assert
        result.Should().NotBeNull();
        result!.Worlds.Count.Should().Be(3);
        result.Worlds[0].Name.Should().Be("Antica");
        result.Worlds[1].Name.Should().Be("Premia");
        result.Worlds[2].Name.Should().Be("Vunira");
    }
}