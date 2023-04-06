using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class UpdateOrCreateCharacterCorrelationsBenchmark
{
    private const string _connectionString = "Server=localhost;Port=5432;Database=local_database2;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(_connectionString).UseSnakeCaseNamingConvention().Options);


    [Benchmark(Baseline = true)]
    public async Task UpdateOrCreateCharacterCorrelationsSeparate()
    {
        await UpdateCharacterCorrelations();
        await CreateCharacterCorrelationsIfNotExist();
    }

    [Benchmark]
    public async Task UpdateOrCreateCharacterCorrelations()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstAsync()).LogoutOrLoginDate;
        var loginCharactersIds = CharactersIds(true);
        var logoutCharactersIds = CharactersIds(false);
        
        var correlationsDataToCreate = loginCharactersIds
            .SelectMany(login => logoutCharactersIds,
                (login, logout) => new { LoginCharacterId = login, LogoutCharacterId = logout });

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

        var existingCharacterCorrelationsPart1 = _dbContext.CharacterCorrelations.Where(cc =>
            characterCorrelationsIdsPart1.Concat(characterCorrelationsIdsPart2).Contains(cc.CorrelationId))
            .Select(c => new { c.LoginCharacterId, c.LogoutCharacterId});
        
        var existingCharacterCorrelationsPart2 = _dbContext.CharacterCorrelations.Where(cc =>
            characterCorrelationsIdsPart1.Concat(characterCorrelationsIdsPart2).Contains(cc.CorrelationId))
            .Select(c => new {LoginCharacterId = c.LogoutCharacterId, LogoutCharacterId = c.LoginCharacterId});
        
        var correlationsDataToInsert = correlationsDataToCreate.Except(existingCharacterCorrelationsPart1).Except(existingCharacterCorrelationsPart2);
     
        var newCorrelations = correlationsDataToInsert
            .Select(cc => new CharacterCorrelation
            {
                LoginCharacterId = cc.LoginCharacterId,
                LogoutCharacterId = cc.LogoutCharacterId,
                NumberOfMatches = 1,
                CreateDate = lastMatchDate,
                LastMatchDate = lastMatchDate
            }).ToList();

        _dbContext.CharacterCorrelations.AddRange(newCorrelations);
        await _dbContext.SaveChangesAsync();
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
    
      private async Task UpdateCharacterCorrelations()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstAsync()).LogoutOrLoginDate;
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
    
    private async Task CreateCharacterCorrelationsIfNotExist()
    {
        var lastMatchDate = (await _dbContext.CharacterActions.FirstAsync()).LogoutOrLoginDate;
        var loginCharactersIds = CharactersIds(true);
        var logoutCharactersIds = CharactersIds(false);
        
        var correlationsDataToCreate = loginCharactersIds
            .SelectMany(login => logoutCharactersIds,
                (login, logout) => new { LoginCharacterId = login, LogoutCharacterId = logout });
        
        var existingCharacterCorrelationsPart1 =
            _dbContext.Characters
                .Where(c => loginCharactersIds.Contains(c.CharacterId))
                .SelectMany(c => c.LoginCharacterCorrelations.Select(cc => new { cc.LoginCharacterId, cc.LogoutCharacterId}));
        
        var existingCharacterCorrelationsPart2 =
            _dbContext.Characters
                .Where(c => logoutCharactersIds.Contains(c.CharacterId))
                .SelectMany(c => c.LoginCharacterCorrelations.Select(cc => new {LoginCharacterId = cc.LogoutCharacterId, LogoutCharacterId = cc.LoginCharacterId}));
  
        var correlationsDataToInsert = correlationsDataToCreate.Except(existingCharacterCorrelationsPart1).Except(existingCharacterCorrelationsPart2);
     
        var newCorrelations = correlationsDataToInsert
            .Select(cc => new CharacterCorrelation
            {
                LoginCharacterId = cc.LoginCharacterId,
                LogoutCharacterId = cc.LogoutCharacterId,
                NumberOfMatches = 1,
                CreateDate = lastMatchDate,
                LastMatchDate = lastMatchDate
            }).ToList();

        _dbContext.CharacterCorrelations.AddRange(newCorrelations);
        await _dbContext.SaveChangesAsync();
    }
}
    
