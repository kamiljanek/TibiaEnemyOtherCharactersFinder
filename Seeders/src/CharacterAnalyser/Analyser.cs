using CharacterAnalyser.ActionRules;
using CharacterAnalyser.ActionRules.Rules;
using CharacterAnalyser.Decorators;
using CharacterAnalyser.Managers;
using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace CharacterAnalyser;

public class Analyser : ActionRule, IAnalyser
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly IAnalyserLogDecorator _logDecorator;
    private readonly CharacterActionsManager _characterActionsManager;
    private readonly WorldScansProcessor _processor;

    public Analyser(ITibiaCharacterFinderDbContext dbContext, IAnalyserLogDecorator logDecorator)
    {
        _dbContext = dbContext;
        _logDecorator = logDecorator;
        _characterActionsManager = new CharacterActionsManager(dbContext);
        _processor = new WorldScansProcessor(dbContext, logDecorator);

    }

    public async Task<List<short>> GetDistinctWorldIdsFromRemainingScans()
    {
        var result = await _dbContext.WorldScans
            .Where(scan => !scan.IsDeleted)
            .GroupBy(scan => scan.WorldId)
            .Where(group => group.Count() >= 2)
            .Select(group => group.Key)
            .OrderBy(id => id)
            .ToListAsync();

        return result;
    }

    public async Task<List<WorldScan>> GetWorldScansToAnalyseAsync(short worldId)
    {
        var result = await _dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .AsNoTracking()
            .ToListAsync();

        return result;
    }

    public async Task Seed(List<WorldScan> twoWorldScans)
    {
        if (IsBroken(new NumberOfWorldScansSpecificAmountRule(twoWorldScans)))
            return;

        if (IsBroken(new TimeBetweenWorldScansCannotBeLongerThanMaxDurationRule(twoWorldScans)) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterActionsManager.GetAndSetLogoutNames(twoWorldScans))) ||
            IsBroken(new CharacterNameListCannotBeEmptyRule(_characterActionsManager.GetAndSetLoginNames(twoWorldScans))))
        {
            await SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            return;
        }

        await _dbContext.ExecuteRawSqlAsync(GenerateQueries.ClearCharacterActions);
        await _dbContext.ExecuteRawSqlAsync(GenerateQueries.UpdateCharactersSetFoundInScanFalse);

        try
        {
            await _logDecorator.Decorate(_characterActionsManager.SeedCharacterActions, twoWorldScans);
            await _processor.ProcessAsync(twoWorldScans);
        }
        finally
        {
            await SoftDeleteWorldScanAsync(twoWorldScans[0].WorldScanId);
            _dbContext.ChangeTracker.Clear();
        }
    }

    private async Task SoftDeleteWorldScanAsync(int scanId)
    {
        await _dbContext.WorldScans
            .Where(ws => ws.WorldScanId == scanId)
            .ExecuteUpdateAsync(update => update.SetProperty(ws => ws.IsDeleted, true));
    }
}