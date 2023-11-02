using TibiaEnemyOtherCharactersFinder.Application.Interfaces;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace CharacterAnalyser.Modules;

public class CharacterManager : ISeeder<List<WorldScan>>
{
    private IReadOnlyList<string> _loginNames;
    private IReadOnlyList<string> _logoutNames;
    private IReadOnlyList<string> _firstScanNames;

    private readonly IRepository _repository;

    public CharacterManager(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Seed(List<WorldScan> twoWorldScans)
    {
        var logoutCharacters = CreateCharactersActionsAsync(_logoutNames, twoWorldScans[0], isOnline: false);
        var loginCharacters = CreateCharactersActionsAsync(_loginNames, twoWorldScans[1], isOnline: true);

        var characterActionsToAdd = logoutCharacters.Concat(loginCharacters);
        await _repository.AddRangeAsync(characterActionsToAdd);

        await _repository.SetCharacterFoundInScanAsync(_firstScanNames, foundInScan: true);
    }

    public IReadOnlyList<string> GetAndSetLoginNames(List<WorldScan> twoWorldScans) => _loginNames = GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToArray();

    public IReadOnlyList<string> GetAndSetLogoutNames(List<WorldScan> twoWorldScans)
    {
        _firstScanNames = GetNames(twoWorldScans[0]);
        var secondScanNames = GetNames(twoWorldScans[1]);
        return _logoutNames = _firstScanNames.Except(secondScanNames).ToArray();

    }

    private static List<CharacterAction> CreateCharactersActionsAsync(IEnumerable<string> names, WorldScan worldScan, bool isOnline)
    {
        var logoutOrLoginDate = DateOnly.FromDateTime(worldScan.ScanCreateDateTime);
        
        return names.Select(name => new CharacterAction()
        {
            CharacterName = name,
            WorldScanId = worldScan.WorldScanId,
            WorldId = worldScan.WorldId,
            IsOnline = isOnline,
            LogoutOrLoginDate = logoutOrLoginDate
        }).ToList();
    }

    private static List<string> GetNames(WorldScan worldScan)
    {
        return worldScan.CharactersOnline.Split("|", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}