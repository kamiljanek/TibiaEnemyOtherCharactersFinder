using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;
using Z.EntityFramework.Plus;

namespace Seeders.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class UpdateCharacterCorrelationsBenchmark
{
    private const string ConnectionString = "Server=localhost;Port=5432;Database=local_database;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(ConnectionString).UseSnakeCaseNamingConvention().Options);
    
    [Benchmark(Baseline = true)]
    public async Task UpdateCharacterCorrelationsEfCore()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).LogoutOrLoginDate;
        var loginCharactersIds = CharactersIds(true);
        var logoutCharactersIds = CharactersIds(false);

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
    
    [Benchmark]
    public async Task UpdateCharacterCorrelationsEfCoreNew()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).LogoutOrLoginDate;
        var loginCharactersIds = CharactersIds(true);
        var logoutCharactersIds = CharactersIds(false);
        
        var existingCharacterCorrelationsIdsPart1 =
            _dbContext.Characters
                .Where(c => loginCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginWorldCorrelations
                    .Where(a => logoutCharactersIds.Contains(a.LogoutCharacterId))).Select(cc => cc.CorrelationId);
        
        var existingCharacterCorrelationsIdsPart2 =
            _dbContext.Characters
                .Where(c => logoutCharactersIds.Contains(c.CharacterId))
                .SelectMany(wc => wc.LoginWorldCorrelations
                    .Where(a => loginCharactersIds.Contains(a.LogoutCharacterId))).Select(cc => cc.CorrelationId);
        
        await _dbContext.CharacterCorrelations
            .Where(cc => existingCharacterCorrelationsIdsPart1.Concat(existingCharacterCorrelationsIdsPart2).Contains(cc.CorrelationId))
            .ExecuteUpdateAsync(update => update
                .SetProperty(c => c.NumberOfMatches, c => c.NumberOfMatches + 1)
                .SetProperty(c => c.LastMatchDate, lastMatchDate));
    }
    
    private IQueryable<int> CharactersIds(bool isOnline)
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
    
