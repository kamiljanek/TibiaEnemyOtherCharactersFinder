using FluentAssertions;
using TibiaEnemyOtherCharactersFinder.Application.Dtos;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace TibiaEnemyOtherCharacterFinder.IntegrationTests.CharacterController;

public class CharacterControllerTests : IClassFixture<TibiaApiFactory>
{
    private const string _controllerBase = "api/character";
    private const string _defaultName = "duzzerah";
    private readonly TibiaApiFactory _factory;

    public CharacterControllerTests(TibiaApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetOtherCharacters_WithRouteParameters_ReturnsOkResult()
    {
        // Arrange
        var client = _factory.CreateClient();
        //SeedData();

        // Act
        var response = await client.GetAsync($"{_controllerBase}/{_defaultName}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetOtherCharacters_WithRouteParameters_ReturnsCorrectData()
    {
        // Arrange
        //SeedData();
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"{_controllerBase}/duzzerah");
        var result = await response.Content.ReadFromJsonAsync<List<CharacterWithCorrelationsResult>>();
        // Assert

        result.Should().NotBeEmpty();
        result.First(c => c.OtherCharacterName == "someName").NumberOfMatches.Should().Be(5);
    }

    [Fact]
    public void DataInDatabaseShouldBeCorrectUsingEFCore()
    {
        using var scope = _factory.Services.CreateScope();
        // Arrange
        var dbContext = scope.ServiceProvider.GetRequiredService<TibiaCharacterFinderDbContext>();

        // Act
        var worlds = dbContext.Worlds.Where(w => w.IsAvailable).ToList();

        // Assert
        worlds.Should().NotBeEmpty();
    }

    //private void SeedData()
    //{
    //    SeedWorlds();
    //    SeedCharacters();
    //    SeedCharacterCorrelaions();
    //}

    //private void SeedWorlds()
    //{
    //    var worlds = new List<World>()
    //    {
    //        new() { WorldId = 1, Name = "Premia", Url = "urlPremia"},
    //        new() { WorldId = 2, Name = "Vunira", Url = "urlVunira"}
    //    };
    //    _repository.AddRangeAsync(worlds);
    //}

    //private void SeedCharacters()
    //{
    //    var characters = new List<Character>()
    //    {
    //        new() { CharacterId = 1, Name = "abargo maewa", WorldId = 1 },
    //        new() { CharacterId = 2, Name = "amy winehousse", WorldId = 1 },
    //        new() { CharacterId = 3, Name = "duzzerah", WorldId = 1 },

    //        new() { CharacterId = 4, Name = "ziomal rafiego", WorldId = 2},
    //        new() { CharacterId = 5, Name = "zanteey", WorldId = 2 },
    //        new() { CharacterId = 6, Name = "artenian", WorldId = 2 },
    //    };
    //    _repository.AddRangeAsync(characters);
    //}

    //private void SeedCharacterCorrelaions()
    //{
    //    var characterCorrelations = new List<CharacterCorrelation>()
    //    {
    //        new() { LoginCharacterId = 1, LogoutCharacterId = 2, NumberOfMatches = 8 },
    //        new() { LoginCharacterId = 3, LogoutCharacterId = 1, NumberOfMatches = 4 },
    //        new() { LoginCharacterId = 3, LogoutCharacterId = 2, NumberOfMatches = 2 },

    //        new() { LoginCharacterId = 4, LogoutCharacterId = 6, NumberOfMatches = 10 },
    //        new() { LoginCharacterId = 6, LogoutCharacterId = 5, NumberOfMatches = 21 },
    //        new() { LoginCharacterId = 5, LogoutCharacterId = 6, NumberOfMatches = 1 },
    //    };
    //    _repository.AddRangeAsync(characterCorrelations);
    //}
}