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
    private readonly CharacterManager _characterManager;
    private readonly CharacterActionsCleaner _characterActionsCleaner;
    private readonly CharacterSeeder _characterSeeder;
    private readonly CharacterCorrelationSeeder _characterCorrelationSeeder;
    private readonly CharacterCorrelationDeleter _characterCorrelationDeleter;

    public List<short> UniqueWorldIds => _uniqueWorldIds;

    public Analyser(IRepository repository,
                             CharacterManager characterManager,
                             CharacterActionsCleaner characterActionsCleaner,
                             CharacterSeeder characterSeeder,
                             CharacterCorrelationSeeder characterCorrelationSeeder,
                             CharacterCorrelationDeleter characterCorrelationDeleter)
    {
        _repository = repository;
        _characterManager = characterManager;
        _characterActionsCleaner = characterActionsCleaner;
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
        Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - start");

        if (IsBroken(new NumberOfWorldScansSpecificAmountRule(twoWorldScans)))
            return;

        if (IsBroken(new TimeBetweenWorldScansCannotBeLongerThanMaxDurationRule(twoWorldScans)) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterManager.GetAndSetLogoutNames(twoWorldScans))) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterManager.GetAndSetLoginNames(twoWorldScans))))
        {
            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - not analysed");
            return;
        }

        await _characterActionsCleaner.ClearAsync();
        await _characterActionsCleaner.ResetFoundInScanInCharactersAsync();
        Console.WriteLine($"{twoWorldScans[0].WorldScanId} (world_id = {twoWorldScans[0].WorldId}) - {DateTime.Now.ToLongTimeString()} - cleared CharacterActions");

        await SeedAndAnalyseCharacters(twoWorldScans);
    }

    private async Task SeedAndAnalyseCharacters(List<WorldScan> twoWorldScans)
    {
        try
        {
            await _characterManager.Seed(twoWorldScans);
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