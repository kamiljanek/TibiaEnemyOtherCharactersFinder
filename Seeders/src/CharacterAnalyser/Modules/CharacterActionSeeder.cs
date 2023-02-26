using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser.Modules;

public class CharacterActionSeeder : ISeeder<List<WorldScan>>
{
    private List<string> _loginNames;
    private List<string> _logoutNames;

    private readonly IRepository _repository;

    public CharacterActionSeeder(IRepository repository)
    {
        _repository = repository;
    }

    public async Task Seed(List<WorldScan> twoWorldScans)
    {
        var logoutCharacters = CreateCharactersActionsAsync(_logoutNames, twoWorldScans[0], isOnline: false);
        var loginCharacters = CreateCharactersActionsAsync(_loginNames, twoWorldScans[1], isOnline: true);

        await _repository.AddRangeAsync(logoutCharacters);
        await _repository.AddRangeAsync(loginCharacters);
    }

    public List<string> GetLoginNames(List<WorldScan> twoWorldScans) => _loginNames = GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToList();

    public List<string> GetLogoutNames(List<WorldScan> twoWorldScans) => _logoutNames = GetNames(twoWorldScans[0]).Except(GetNames(twoWorldScans[1])).ToList();

    private static List<CharacterAction> CreateCharactersActionsAsync(List<string> names, WorldScan worldScan, bool isOnline) =>
        names.Select(name => new CharacterAction()
        {
            CharacterName = name,
            WorldScanId = worldScan.WorldScanId,
            WorldId = worldScan.WorldId,
            IsOnline = isOnline,
            LogoutOrLoginDate = DateOnly.FromDateTime(worldScan.ScanCreateDateTime)
        }).ToList();

    private static List<string> GetNames(WorldScan worldScan)
    {
        var names = worldScan.CharactersOnline.Split("|").ToList();
        names.RemoveAll(string.IsNullOrWhiteSpace);
        // UNDONE: jak przemieli wszystkie WorldScany po tym deployu to można usunąć poniższą linijkę
        return names.ConvertAll(d => d.ToLower());
    }
}