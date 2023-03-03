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
public class RemoveCharacterCorrelationsBenchmark
{
    private const string ConnectionString = "Server=localhost;Port=5432;Database=local_database;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(ConnectionString).UseSnakeCaseNamingConvention().Options);
    
    [Benchmark(Baseline = true)]
    public async Task RemoveCharacterCorrelationsEfCore()
    {
        var charactersIdsOnline = CharactersIds(true).ToArray();
        var charactersIdsOffline = CharactersIds(false).ToArray();

        await _dbContext.CharacterCorrelations
            .Where(c => charactersIdsOnline.Contains(c.LoginCharacterId) && charactersIdsOnline.Contains(c.LogoutCharacterId))
            .ExecuteDeleteAsync();
        
        await _dbContext.CharacterCorrelations
            .Where(c => charactersIdsOffline.Contains(c.LoginCharacterId) && charactersIdsOffline.Contains(c.LogoutCharacterId))
            .ExecuteDeleteAsync();
    }
    
    [Benchmark]
    public async Task RemoveCharacterCorrelationsEfCoreUnsegregate()
    {
        var charactersIdsOnline = CharactersIds(true).ToArray();

        await _dbContext.CharacterCorrelations
            .Where(c => charactersIdsOnline.Contains(c.LoginCharacterId) && charactersIdsOnline.Contains(c.LogoutCharacterId))
            .ExecuteDeleteAsync();
        
        var charactersIdsOffline = CharactersIds(false).ToArray();
        
        await _dbContext.CharacterCorrelations
            .Where(c => charactersIdsOffline.Contains(c.LoginCharacterId) && charactersIdsOffline.Contains(c.LogoutCharacterId))
            .ExecuteDeleteAsync();
    }
    
    [Benchmark]
    public async Task RemoveCharacterCorrelationsSql()
    {
        await _dbContext.Database.ExecuteSqlRawAsync(GenerateQueries.NpgsqlDeleteCharacterCorrelationIfCorrelationExistInFirstScan);
        await _dbContext.Database.ExecuteSqlRawAsync(GenerateQueries.NpgsqlDeleteCharacterCorrelationIfCorrelationExistInSecondScan);
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
    
