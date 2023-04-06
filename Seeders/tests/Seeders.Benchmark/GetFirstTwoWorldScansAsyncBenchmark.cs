using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace Seeders.Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class GetFirstTwoWorldScansAsyncBenchmark
{
    private const string _connectionString = "Server=localhost;Port=5432;Database=local_database;User Id=sa;Password=pass;";

    private readonly TibiaCharacterFinderDbContext _dbContext = new (new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(_connectionString).UseSnakeCaseNamingConvention().Options);
    
    [Benchmark(Baseline = true)]
    [Arguments(1)]
    public Task<List<WorldScan>> GetFirstTwoWorldScansAsyncAsTracking(short worldId)
    {
        return Task.FromResult(_dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .AsTracking()
            .ToList());
    }
    
    [Benchmark]
    [Arguments(1)]
    public Task<List<WorldScan>> GetFirstTwoWorldScansAsyncAsNoTracking(short worldId)
    {
        return Task.FromResult(_dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .AsNoTracking()
            .ToList());
    }
    
    [Benchmark]
    [Arguments(1)]
    public Task<List<WorldScan>> GetFirstTwoWorldScansAsync(short worldId)
    {
        return Task.FromResult(_dbContext.WorldScans
            .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
            .OrderBy(scan => scan.ScanCreateDateTime)
            .Take(2)
            .ToList());
    }
}
    
