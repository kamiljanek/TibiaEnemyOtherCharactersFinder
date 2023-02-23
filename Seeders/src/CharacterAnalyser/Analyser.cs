using CharacterAnalyser.ActionRules;
using CharacterAnalyser.ActionRules.Rules;
using CharacterAnalyser.Modules;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser;

public class Analyser : ActionRule, IAnalyser
{
    private List<short> _uniqueWorldIds = new();

    private readonly IRepository _repository;
    private readonly CharacterActionSeeder _characterActionSeeder;
    private readonly CharacterAnalyserCleaner _characterAnalyserCleaner;
    private readonly CharacterSeeder _characterSeeder;
    private readonly CharacterCorrelationSeeder _characterCorrelationSeeder;
    private readonly CharacterCorrelationDeleter _characterCorrelationDeleter;

    public List<short> UniqueWorldIds => _uniqueWorldIds;

    public Analyser(IRepository repository,
                             CharacterActionSeeder characterActionSeeder,
                             CharacterAnalyserCleaner characterAnalyserCleaner,
                             CharacterSeeder characterSeeder,
                             CharacterCorrelationSeeder characterCorrelationSeeder,
                             CharacterCorrelationDeleter characterCorrelationDeleter)
    {
        _repository = repository;
        _characterActionSeeder = characterActionSeeder;
        _characterAnalyserCleaner = characterAnalyserCleaner;
        _characterSeeder = characterSeeder;
        _characterCorrelationSeeder = characterCorrelationSeeder;
        _characterCorrelationDeleter = characterCorrelationDeleter;
    }

    public async Task<bool> HasDataToAnalyse()
    {
        var availableWorldIds = await _repository.GetAvailableWorldIdsFromWorldScansAsync();
        _uniqueWorldIds = await _repository.GetDistinctWorldIdsFromWorldScansAsync();

        return availableWorldIds.Count > _uniqueWorldIds.Count;
    }

    public async Task<List<WorldScan>> GetWorldScansToAnalyseAsync(short worldId)
    {
        return await _repository.GetFirstTwoWorldScansAsync(worldId);
    }

    public async Task Seed(List<WorldScan> twoWorldScans)
    {
        if (IsBroken(new NumberOfWorldScansSpecificAmountRule(twoWorldScans)))
            return;

        if (IsBroken(new TimeBetweenWorldScansCannotBeLongerThanMaxDurationRule(twoWorldScans)) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterActionSeeder.GetLogoutNames(twoWorldScans))) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterActionSeeder.GetLoginNames(twoWorldScans))))
        {
            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0]);
            return;
        }

        await _characterAnalyserCleaner.ClearCharacterActionsAsync();
        await AnalizeCharactersAndSeed(twoWorldScans);

        Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId})");
    }

    private async Task AnalizeCharactersAndSeed(List<WorldScan> twoWorldScans)
    {
        try
        {
            await _characterActionSeeder.Seed(twoWorldScans);
            await _characterSeeder.Seed();
            await _characterCorrelationSeeder.Seed();

            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0]);
            
            await _characterCorrelationDeleter.Delete();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}