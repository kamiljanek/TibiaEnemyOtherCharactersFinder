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
public class NumberOfAvailableWorldScansBenchmark
{
    private const string ConnectionString = "Server=localhost;Port=5432;Database=local_database;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(ConnectionString).UseSnakeCaseNamingConvention().Options);
    
    [Benchmark(Baseline = true)]
    public Task<int> NumberOfAvailableWorldScansAsync()
    {
        return Task.FromResult(_dbContext.WorldScans
            .Count(scan => !scan.IsDeleted));
    }
    
    [Benchmark]
    public int NumberOfAvailableWorldScans()
    {
        return _dbContext.WorldScans
            .Count(scan => !scan.IsDeleted);
    }
    

}
    
