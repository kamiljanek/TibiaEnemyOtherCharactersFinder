using CharacterAnalyser.ActionRules;
using CharacterAnalyser.ActionRules.Rules;
using CharacterAnalyser.Decorators;
using CharacterAnalyser.Managers;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Application.Persistence;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;

namespace CharacterAnalyser;

public class Analyser : ActionRule, IAnalyser
{
    private List<short> _uniqueWorldIds = new();

    private readonly IRepository _repository;
    private readonly IAnalyserLogDecorator _logDecorator;
    private readonly CharacterActionsManager _characterActionsManager;

    public List<short> UniqueWorldIds => _uniqueWorldIds;

    public Analyser(IRepository repository, IAnalyserLogDecorator logDecorator)
    {
        _repository = repository;
        _logDecorator = logDecorator;
        _characterActionsManager = new CharacterActionsManager(repository);
    }

    public async Task<bool> HasDataToAnalyse()
    {
        var availableWorldIds = _repository.NumberOfAvailableWorldScans();
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
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterActionsManager.GetAndSetLogoutNames(twoWorldScans))) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterActionsManager.GetAndSetLoginNames(twoWorldScans))))
        {
            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            return;
        }

        await ClearCharacterActionsAsync();
        await ResetFoundInScanOnCharactersAsync();

        try
        {
            await SeedAndAnalyseCharacters(twoWorldScans);
        }
        finally
        {
            await _repository.SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            await _repository.ClearChangeTracker();
        }
    }


    private async Task SeedAndAnalyseCharacters(List<WorldScan> twoWorldScans)
    {
        await _logDecorator.Decorate(_characterActionsManager.SeedCharacterActions, twoWorldScans);
        await _logDecorator.Decorate(SeedCharacters, twoWorldScans);
        await _logDecorator.Decorate(_repository.UpdateCorrelationsIfExistAsync, twoWorldScans);
        await _logDecorator.Decorate(_repository.CreateCorrelationsIfNotExistAsync, twoWorldScans);
        await _logDecorator.Decorate(_repository.RemoveImposibleCorrelationsAsync, twoWorldScans);
    }

    private async Task SeedCharacters()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.CreateCharactersIfNotExists);
    }

    private async Task ClearCharacterActionsAsync()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.ClearCharacterActions);
    }

    private async Task ResetFoundInScanOnCharactersAsync()
    {
        await _repository.ExecuteRawSqlAsync(GenerateQueries.UpdateCharactersSetFoundInScanFalse);
    }
}