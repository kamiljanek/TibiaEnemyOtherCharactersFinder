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
        var availableWorldIds = await _repository.NumberOfAvailableWorldScansAsync();
        _uniqueWorldIds = await _repository.GetDistinctWorldIdsFromWorldScansAsync();

        return availableWorldIds > _uniqueWorldIds.Count;
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
            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - not analysed");
            return;
        }

        Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - start");

        await _characterAnalyserCleaner.ClearCharacterActionsAsync();
        Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - cleared CharacterActions");

        await AnalizeCharactersAndSeed(twoWorldScans);
        Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()}");
    }

    private async Task AnalizeCharactersAndSeed(List<WorldScan> twoWorldScans)
    {
        try
        {
            await _characterActionSeeder.Seed(twoWorldScans);
            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - seeded CharacterActions");

            await _characterSeeder.Seed();
            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - seeded Characters");
            
            await _characterCorrelationSeeder.Seed();

            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - softDeleted Characters");
            
            await _characterCorrelationDeleter.Delete();
            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - deleted CharacterCorrelations");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}