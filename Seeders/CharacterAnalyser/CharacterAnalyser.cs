using CharacterAnalyser.ActionRules;
using CharacterAnalyser.ActionRules.Rules;
using CharacterAnalyser.Modules;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Application.Dapper;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser;

public class CharacterAnalyser : ActionRule, ICharacterAnalyser
{
    private HashSet<short> _wolrdIds = new();
    private List<string> _logoutNames = new();
    private List<string> _loginNames = new();

    private readonly IRepository _repository;
    private readonly CharacterAnalyserCleaner _cleaner;
    private readonly CharacterSeeder _characterSeeder;
    private readonly CharacterCorrelationSeeder _characterCorrelationSeeder;

    public CharacterAnalyser(IDapperConnectionProvider connectionProvider, IRepository repository)
    {
        _repository = repository;
        _cleaner = new CharacterAnalyserCleaner(connectionProvider);
        _characterSeeder = new CharacterSeeder(connectionProvider);
        _characterCorrelationSeeder = new CharacterCorrelationSeeder(connectionProvider);
    }

    public async Task SetProperties() => _wolrdIds = await _repository.GetDistinctWorldIdsFromWorldScansAsync();

    public async Task Seed()
    {
        foreach (var worldId in _wolrdIds)
        {
            var twoWorldScans = await _repository.GetFirstTwoWorldScansAsync(worldId);

            if (IsBroken(new WorldScansSpecificAmountOfElementsRule(twoWorldScans)))
                continue;
            if (IsBroken(new TimeBetweenWorldScansCannotBeLongerThanMaxDurationRule(twoWorldScans)) ||
                IsBroken(new CharacterNameListCannotBeEmpty(_logoutNames = GetLogoutNames(twoWorldScans))) ||
                IsBroken(new CharacterNameListCannotBeEmpty(_loginNames = GetLoginNames(twoWorldScans))))
            {
                await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0]);
                continue;
            }
            await _cleaner.ClearCharacterActionsAsync();

            await AnalizeCharactersAndSeed(twoWorldScans);

            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId})");
        }
    }

    private async Task AnalizeCharactersAndSeed(List<WorldScan> twoWorldScans)
    {
        try
        {
            var characterActionSeeder = new CharacterActionSeeder(_repository, _logoutNames, _loginNames, twoWorldScans);

            await characterActionSeeder.Seed();
            await _characterSeeder.Seed();
            await _characterCorrelationSeeder.Seed();

            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0]);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private List<string> GetLoginNames(List<WorldScan> twoWorldScans) => GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToList();

    private List<string> GetLogoutNames(List<WorldScan> twoWorldScans) => GetNames(twoWorldScans[0]).Except(GetNames(twoWorldScans[1])).ToList();

    private List<string> GetNames(WorldScan worldScan)
    {
        var names = worldScan.CharactersOnline.Split("|").ToList();
        names.RemoveAll(string.IsNullOrWhiteSpace);
        names = names.ConvertAll(d => d.ToLower());
        return names;
    }
}