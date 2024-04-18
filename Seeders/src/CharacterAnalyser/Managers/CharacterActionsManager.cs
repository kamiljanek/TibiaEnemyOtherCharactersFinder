using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace CharacterAnalyser.Managers;

public class CharacterActionsManager
{
    private IReadOnlyList<string> _loginNames;
    private IReadOnlyList<string> _logoutNames;
    private IReadOnlyList<string> _firstScanNames;

    private readonly ITibiaCharacterFinderDbContext _dbContext;

    public CharacterActionsManager(ITibiaCharacterFinderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedCharacterActions(List<WorldScan> twoWorldScans)
    {
        var logoutCharacters = CreateCharactersActionsAsync(_logoutNames, twoWorldScans[0], isOnline: false);
        var loginCharacters = CreateCharactersActionsAsync(_loginNames, twoWorldScans[1], isOnline: true);

        var characterActionsToAdd = logoutCharacters.Concat(loginCharacters);
        await _dbContext.CharacterActions.AddRangeAsync(characterActionsToAdd);
        await _dbContext.SaveChangesAsync();

        // Take only 300 shuffled elements because database needs too much time to execute delete ImposibleCorrelations
        await _dbContext.Characters
            .Where(ch => _firstScanNames.Take(300).Contains(ch.Name))
            .ExecuteUpdateAsync(update => update.SetProperty(c => c.FoundInScan, c => true));
    }

    public IReadOnlyList<string> GetAndSetLoginNames(List<WorldScan> twoWorldScans)
    {
        _loginNames = GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToArray();
        return _loginNames;
    }

    public IReadOnlyList<string> GetAndSetLogoutNames(List<WorldScan> twoWorldScans)
    {
        // Shuffle "_firstScanNames" for delete random ImposibleCorrelations
        var random = new Random();
        _firstScanNames = GetNames(twoWorldScans[0]).OrderBy(c => random.Next()).ToArray();
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