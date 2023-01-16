using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterActionSeeder : ISeeder
{
    private readonly IRepository _repository;
    private readonly List<string> _logoutNames;
    private readonly List<string> _loginNames;
    private readonly List<WorldScan> _twoWorldScans;

    public CharacterActionSeeder(IRepository repository, List<string> logoutNames, List<string> loginNames, List<WorldScan> twoWorldScans)
    {
        _repository = repository;
        _logoutNames = logoutNames;
        _loginNames = loginNames;
        _twoWorldScans = twoWorldScans;
    }

    public async Task Seed()
    {
        await SeedLogoutCharactersActionsAsync(_logoutNames, _twoWorldScans[0]);
        await SeedLoginCharactersActionsAsync(_loginNames, _twoWorldScans[1]);
    }

    private async Task SeedLogoutCharactersActionsAsync(List<string> logoutNames, WorldScan worldScan)
    {
        var logoutCharacters = logoutNames.Select(name => CreateCharacterLogoutOrLogin(name, isOnline: false, worldScan)).ToList();
        await _repository.AddCharactersActionsAsync(logoutCharacters);
    }

    private async Task SeedLoginCharactersActionsAsync(List<string> loginNames, WorldScan worldScan)
    {
        var loginCharacters = loginNames.Select(name => CreateCharacterLogoutOrLogin(name, isOnline: true, worldScan)).ToList();
        await _repository.AddCharactersActionsAsync(loginCharacters);
    }

    private CharacterAction CreateCharacterLogoutOrLogin(string characterName, bool isOnline, WorldScan worldScan)
    {
        return new CharacterAction()
        {
            CharacterName = characterName,
            WorldScanId = worldScan.WorldScanId,
            WorldId = worldScan.WorldId,
            IsOnline = isOnline,
            LogoutOrLoginDate = DateOnly.FromDateTime(worldScan.ScanCreateDateTime)
        };
    }
}