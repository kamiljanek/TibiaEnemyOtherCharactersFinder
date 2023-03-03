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
public class CreateCharacterIfNotExistBenchmark
{
    private const string ConnectionString = "Server=localhost;Port=5432;Database=local_database;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(ConnectionString).UseSnakeCaseNamingConvention().Options);
    
    [Benchmark(Baseline = true)]
    public async Task CreateCharacterIfNotExistSql()
    {
        await _dbContext.Database.ExecuteSqlRawAsync(GenerateQueries.NpgsqlCreateCharacterIfNotExist);
    }
    
    [Benchmark]
    public async Task CreateCharacterIfNotExistEfCore()
    {
        var worldId = (await _dbContext.CharacterActions.FirstOrDefaultAsync()).WorldId;

        await _dbContext.Characters
            .BulkInsertAsync(CharactersNames().Select(name => new Character{Name = name, WorldId = worldId}), 
                options =>
                {
                    options.InsertIfNotExists = true;
                    options.ColumnPrimaryKeyExpression = c => c.Name;
                });
    }
    
    private IQueryable<string> CharactersNames()
    {
        return _dbContext.CharacterActions
            .Select(ca => ca.CharacterName)
            .Distinct();
    }

}
    
