using CharacterAnalyser.Decorators;
using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace CharacterAnalyser.Managers;

public class WorldScansProcessor
{
    private readonly ITibiaCharacterFinderDbContext _dbContext;
    private readonly IAnalyserLogDecorator _logDecorator;

    public WorldScansProcessor(ITibiaCharacterFinderDbContext dbContext,
        IAnalyserLogDecorator logDecorator)
    {
        _dbContext = dbContext;
        _logDecorator = logDecorator;
    }

    public async Task ProcessAsync(List<WorldScan> twoWorldScans)
    {
        await _logDecorator.Decorate(SeedCharacters, twoWorldScans);
        await _logDecorator.Decorate(UpdateCorrelationsIfExistAsync, twoWorldScans);
        await _logDecorator.Decorate(CreateCorrelationsIfNotExistAsync, twoWorldScans);
        await _logDecorator.Decorate(RemoveImposibleCorrelationsAsync, twoWorldScans);
    }

    private async Task SeedCharacters()
    {
        await _dbContext.ExecuteRawSqlAsync(GenerateQueries.CreateCharactersIfNotExists);
    }

    private async Task UpdateCorrelationsIfExistAsync()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).LogoutOrLoginDate;
        var loginCharactersIds = GetCharactersIdsBasedOnCharacterActions(isOnline: true);
        var logoutCharactersIds = GetCharactersIdsBasedOnCharacterActions(isOnline: false);

        var characterCorrelationsIdsPart1 =  _dbContext.CharacterCorrelations
            .Where(c => loginCharactersIds.Contains(c.LoginCharacterId) && logoutCharactersIds.Contains(c.LogoutCharacterId))
            .Select(cc => cc.CorrelationId);

        var characterCorrelationsIdsPart2 =  _dbContext.CharacterCorrelations
            .Where(c => logoutCharactersIds.Contains(c.LoginCharacterId) && loginCharactersIds.Contains(c.LogoutCharacterId))
            .Select(cc => cc.CorrelationId);

        await _dbContext.CharacterCorrelations
            .Where(cc => characterCorrelationsIdsPart1.Concat(characterCorrelationsIdsPart2).Contains(cc.CorrelationId))
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.NumberOfMatches, c => c.NumberOfMatches + 1)
                .SetProperty(c => c.LastMatchDate, lastMatchDate));
    }

    private async Task CreateCorrelationsIfNotExistAsync()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).LogoutOrLoginDate;
        var loginCharactersIds = GetCharactersIdsBasedOnCharacterActions(isOnline: true);
        var logoutCharactersIds = GetCharactersIdsBasedOnCharacterActions(isOnline: false);

        var correlationsDataToCreate = loginCharactersIds
            .SelectMany(login => logoutCharactersIds,
                (login, logout) => new { LoginCharacterId = login, LogoutCharacterId = logout });

        var existingCharacterCorrelationsPart1 =
            _dbContext.Characters
                .Where(c => loginCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginCharacterCorrelations.Select(cc => new {LoginCharacterId = cc.LoginCharacterId, LogoutCharacterId = cc.LogoutCharacterId}));

        var existingCharacterCorrelationsPart2 =
            _dbContext.Characters
                .Where(c => logoutCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginCharacterCorrelations.Select(cc => new {LoginCharacterId = cc.LogoutCharacterId, LogoutCharacterId = cc.LoginCharacterId}));

        var correlationsDataToInsert = correlationsDataToCreate.Except(existingCharacterCorrelationsPart1).Except(existingCharacterCorrelationsPart2);

        var newCorrelations = correlationsDataToInsert
            .Select(cc => new CharacterCorrelation
            {
                LoginCharacterId = cc.LoginCharacterId,
                LogoutCharacterId = cc.LogoutCharacterId,
                NumberOfMatches = 1,
                CreateDate = lastMatchDate,
                LastMatchDate = lastMatchDate
            });

        await _dbContext.CharacterCorrelations.AddRangeAsync(newCorrelations);
        await _dbContext.SaveChangesAsync();
    }

    private async Task RemoveImposibleCorrelationsAsync()
    {
        var onlinePlayersAtSameTime = _dbContext.Characters.Where(c => c.FoundInScan).Select(c => c.CharacterId);

        await _dbContext.CharacterCorrelations
            .Where(cc => onlinePlayersAtSameTime.Contains(cc.LoginCharacterId) && onlinePlayersAtSameTime.Contains(cc.LogoutCharacterId))
            .ExecuteDeleteAsync();
    }

    private IQueryable<int> GetCharactersIdsBasedOnCharacterActions(bool isOnline)
    {
        return _dbContext.Characters
            .Where(c =>
                _dbContext.CharacterActions
                    .Where(ca => ca.IsOnline == isOnline)
                    .Select(ca => ca.CharacterName)
                    .Distinct().Contains(c.Name))
            .Select(ca => ca.CharacterId);
    }
}