using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class RemoveCharacterCorrelationsBenchmark
{
    private const string _removeWithCte = @"WITH online_characters AS 
                                        (SELECT character_id FROM characters c WHERE found_in_scan = true)

                                        DELETE FROM character_correlations
                                            WHERE logout_character_id IN
                                        (SELECT character_id
                                            FROM online_characters)

                                        AND login_character_id IN
                                        (SELECT character_id
                                            FROM online_characters)";

    private const string _removeWithoutCte = @"WITH online_characters AS 
                                        (SELECT character_id FROM characters c WHERE found_in_scan = true)

                                        DELETE FROM character_correlations
                                            WHERE logout_character_id IN
                                        (SELECT character_id
                                            FROM characters WHERE found_in_scan = true)

                                        AND login_character_id IN
                                        (SELECT character_id
                                            FROM characters WHERE found_in_scan = true)";

    private const string _connectionString = "Server=localhost;Port=5432;Database=local_database;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(_connectionString).UseSnakeCaseNamingConvention().Options);
    
    [Benchmark(Baseline = true)]
    public async Task RemoveCharacterCorrelationsEfCore()
    {
        await _dbContext.CharacterCorrelations
            .Where(c => CharactersIdsInScan().Contains(c.LoginCharacterId) && CharactersIdsInScan().Contains(c.LogoutCharacterId))
            .ExecuteDeleteAsync();
    }
    
    [Benchmark]
    public async Task RemoveCharacterCorrelationsSqlWithEtc()
    {
        await _dbContext.Database.ExecuteSqlRawAsync(_removeWithCte);
    }

    [Benchmark]
    public async Task RemoveCharacterCorrelationsSqlWithoutEtc()
    {
        await _dbContext.Database.ExecuteSqlRawAsync(_removeWithoutCte);
    }

    private IQueryable<int> CharactersIdsInScan()
    {
        return _dbContext.Characters
            .Where(c => c.FoundInScan)
            .Select(c => c.CharacterId);
    }
}
    
