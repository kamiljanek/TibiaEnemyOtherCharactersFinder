using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharactersController;

public class GetFilteredCharactersTests : CharactersControllerTestTemplate, IClassFixture<TibiaApiFactory>
{
    private const string ControllerBase = "api/tibia-eocf/v1/characters";
    private readonly TibiaApiFactory _factory;

    public GetFilteredCharactersTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetFilteredCharactersEndpoint_WithParametersThatFitsToDataInDatabase_ShouldReturnsCorrectData()
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = "ma";
        var page = 1;
        var pageSize = 10;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var response = await client.GetAsync($"{ControllerBase}{additionalParameters}");
        var result = await response.Content.ReadFromJsonAsync<FilteredCharactersDto>();

        // Assert
        result.Should().NotBeNull();
        result!.TotalCount.Should().Be(2);
        result.Names[0].Should().Be("abargo maewa");
        result.Names[1].Should().Be("ziomal rafiego");
    }
    
    [Fact]
    public async Task GetFilteredCharactersEndpoint_WithSearchTextThatNotFoundInDatabase_ShouldReturnStatusNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var searchText = "abcde";
        var page = 1;
        var pageSize = 10;
        var additionalParameters = $"?{nameof(searchText)}={searchText}&{nameof(page)}={page}&{nameof(pageSize)}={pageSize}";

        // Act
        var response = await client.GetAsync($"{ControllerBase}{additionalParameters}");
        var content = await response.Content.ReadAsStringAsync();
        var contentDeserialized = JsonConvert.DeserializeObject<FilteredCharactersDto>(content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        contentDeserialized!.TotalCount.Should().Be(0);
    }

    [Theory]
    [MemberData(nameof(GetInvalidLengthRouteParameters))]
    public async Task GetFilteredCharactersEndpoint_WithInvalidLenghtSearchText_ShouldReturnsStatusBadRequest(string parameter)
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
    public async Task GetFilteredCharactersEndpoint_WithUnacceptableCharactersInSearchText_ShouldReturnsStatusBadRequest(string parameter)
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
    public async Task GetFilteredCharactersEndpoint_WithUnacceptablePage_ShouldReturnsStatusBadRequest(int parameter)
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
    public async Task GetFilteredCharactersEndpoint_WithUnacceptablePageSize_ShouldReturnsStatusBadRequest(int parameter)
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