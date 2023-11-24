using System.Net;
using FluentAssertions;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;

public class GetFilteredCharactersPromptTests : CharactersControllerTestTemplate, IClassFixture<TibiaApiFactory>
{
    private const string ControllerBase = "api/tibia-eocf/v1/characters/prompt";
    private readonly TibiaApiFactory _factory;

    public GetFilteredCharactersPromptTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFilteredCharactersPromptEndpoint_WithParametersThatFitsToDataInDatabase_ShouldReturnsCorrectData()
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = "abarg";
        var page = 1;
        var pageSize = 10;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var response = await client.GetAsync($"{ControllerBase}{additionalParameters}");
        var result = await response.Content.ReadFromJsonAsync<List<string>>();

        // Assert
        result.Should().NotBeNull();
        result!.Count.Should().BeGreaterThan(0);
        result[0].Should().Be("abargo maewa");
    }
    
    [Theory]
    [MemberData(nameof(GetInvalidLengthRouteParameters))]
    public async Task GetFilteredCharactersPromptEndpoint_WithInvalidLenghtSearchText_ShouldReturnsStatusBadRequest(string parameter)
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = parameter;
        var page = 1;
        var pageSize = 10;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var result = await client.GetAsync($"{ControllerBase}{additionalParameters}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(GetUnacceptableRouteParameters))]
    public async Task GetFilteredCharactersPromptEndpoint_WithUnacceptableCharactersInSearchText_ShouldReturnsStatusBadRequest(string parameter)
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = parameter;
        var page = 1;
        var pageSize = 10;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var result = await client.GetAsync($"{ControllerBase}{additionalParameters}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(GetUnacceptablePages))]
    public async Task GetFilteredCharactersPromptEndpoint_WithUnacceptablePage_ShouldReturnsStatusBadRequest(int parameter)
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = "aba";
        var page = parameter;
        var pageSize = 10;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var result = await client.GetAsync($"{ControllerBase}{additionalParameters}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [MemberData(nameof(GetUnacceptablePageSizes))]
    public async Task GetFilteredCharactersPromptEndpoint_WithUnacceptablePageSize_ShouldReturnsStatusBadRequest(int parameter)
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = "aba";
        var page = 1;
        var pageSize = parameter;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var result = await client.GetAsync($"{ControllerBase}{additionalParameters}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}