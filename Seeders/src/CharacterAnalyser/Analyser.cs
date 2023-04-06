﻿using System.Diagnostics;
using CharacterAnalyser.ActionRules;
using CharacterAnalyser.ActionRules.Rules;
using CharacterAnalyser.Modules;
using Microsoft.Extensions.Logging;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Services;

namespace CharacterAnalyser;

public class Analyser : ActionRule, IAnalyser
{
    private List<short> _uniqueWorldIds = new();

    private readonly IRepository _repository;
    private readonly ILogger<Analyser> _logger;
    private readonly CharacterManager _characterManager;
    private readonly CharacterActionsCleaner _characterActionsCleaner;
    private readonly CharacterSeeder _characterSeeder;
    private readonly CharacterCorrelationUpdater _characterCorrelationUpdater;
    private readonly CharacterCorrelationSeeder _characterCorrelationSeeder;
    private readonly CharacterCorrelationDeleter _characterCorrelationDeleter;

    public List<short> UniqueWorldIds => _uniqueWorldIds;

    public Analyser(IRepository repository,
        ILogger<Analyser> logger,
        CharacterManager characterManager,
        CharacterActionsCleaner characterActionsCleaner,
        CharacterSeeder characterSeeder,
        CharacterCorrelationUpdater characterCorrelationUpdater,
        CharacterCorrelationSeeder characterCorrelationSeeder,
        CharacterCorrelationDeleter characterCorrelationDeleter)
    {
        _repository = repository;
        _logger = logger;
        _characterManager = characterManager;
        _characterActionsCleaner = characterActionsCleaner;
        _characterSeeder = characterSeeder;
        _characterCorrelationUpdater = characterCorrelationUpdater;
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
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterManager.GetAndSetLogoutNames(twoWorldScans))) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterManager.GetAndSetLoginNames(twoWorldScans))))
        {
            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            return;
        }

        await _characterActionsCleaner.ClearAsync();
        await _characterActionsCleaner.ResetFoundInScanOnCharactersAsync();

        await SeedAndAnalyseCharacters(twoWorldScans);
    }

    private async Task SeedAndAnalyseCharacters(List<WorldScan> twoWorldScans)
    {
            await Decorate(SeedCharacterActions, twoWorldScans);
            await Decorate(SeedCharacters, twoWorldScans);
            await Decorate(UpdateCharacterCorrelations, twoWorldScans);
            await Decorate(CreateCharacterCorrelations, twoWorldScans);
            await Decorate(SoftDeleteWorldScan, twoWorldScans);
            await Decorate(DeleteCharacterCorrelations, twoWorldScans);
    }

    private async Task Decorate<T>(Func<T, Task> function, T parameter)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await function.Invoke(parameter);
            stopwatch.Stop();
            _logger.LogInformation("WorldScan({worldScanId}) - World({worldId}) Execution time {methodName}: {time} ms.",
                (parameter as List<WorldScan>)![0].WorldScanId, (parameter as List<WorldScan>)![0].WorldId, function.Method.Name,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "WorldScan({worldScanId}) - World({WorldId}) - Execution {methodName} couse error",
                (parameter as List<WorldScan>)![0].WorldScanId, (parameter as World)!.WorldId, function.Method.Name);
        }
    }

    private async Task SoftDeleteWorldScan(List<WorldScan> twoWorldScans)
    {
        await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
    }

    private async Task DeleteCharacterCorrelations(List<WorldScan> twoWorldScans)
    {
        await _characterCorrelationDeleter.Delete();
    }

    private async Task CreateCharacterCorrelations(List<WorldScan> twoWorldScans)
    {
        await _characterCorrelationSeeder.Seed();
    }

    private async Task UpdateCharacterCorrelations(List<WorldScan> twoWorldScans)
    {
        await _characterCorrelationUpdater.Seed();
    }

    private async Task SeedCharacters(List<WorldScan> twoWorldScans)
    {
        await _characterSeeder.Seed();
    }

    private async Task SeedCharacterActions(List<WorldScan> twoWorldScans)
    {
        await _characterManager.Seed(twoWorldScans);
    }
}