using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using TibiaEnemyOtherCharactersFinder.Domain.Entities;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Persistence;

namespace Seeders.Benchmark;

[MemoryDiagnoser]
// [Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class CharacterActionSeederBenchmark
{
    [Benchmark(Baseline = true)]
    public async Task CharacterActionSeederDelete()
    {
        var connectionString = "Server=localhost;Port=5432;Database=local_database2;User Id=sa;Password=pass;";
        TibiaCharacterFinderDbContext dbContext = new(new DbContextOptionsBuilder<TibiaCharacterFinderDbContext>()
            .UseNpgsql(connectionString).UseSnakeCaseNamingConvention().Options);

        List<WorldScan> GetFirstTwoWorldScansAsync(short worldId)
        {
            return dbContext.WorldScans
                .Where(scan => scan.WorldId == worldId && !scan.IsDeleted)
                .OrderBy(scan => scan.ScanCreateDateTime)
                .Take(2)
                .AsNoTracking()
                .ToList();
        }

        var twoWorldScans = GetFirstTwoWorldScansAsync(2);
        var loginNames = GetLoginNames(twoWorldScans);
        var logoutNames = GetLogoutNames(twoWorldScans);
        var logoutCharacters = CreateCharactersActionsAsync(logoutNames, twoWorldScans[0], false);
        var loginCharacters = CreateCharactersActionsAsync(loginNames, twoWorldScans[1], true);

        dbContext.CharacterActions.AddRange(logoutCharacters.Concat(loginCharacters));

        await dbContext.SaveChangesAsync();

        await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM character_actions");
    }


    private List<string> GetLoginNames(List<WorldScan> twoWorldScans)
    {
        return GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToList();
    }

    private List<string> GetLogoutNames(List<WorldScan> twoWorldScans)
    {
        return GetNames(twoWorldScans[0]).Except(GetNames(twoWorldScans[1])).ToList();
    }


    private static List<string> GetNames(WorldScan worldScan)
    {
        var names = worldScan.CharactersOnline.Split("|").ToList();
        names.RemoveAll(string.IsNullOrWhiteSpace);
        return names.ConvertAll(d => d.ToLower());
    }

    private static List<CharacterAction> CreateCharactersActionsAsync(List<string> names, WorldScan worldScan, bool isOnline)
    {
        var logoutOrLoginDate = DateOnly.FromDateTime(worldScan.ScanCreateDateTime);
        
        return names.Select(name => new CharacterAction
        {
            CharacterName = name,
            WorldScanId = worldScan.WorldScanId,
            WorldId = worldScan.WorldId,
            IsOnline = isOnline,
            LogoutOrLoginDate = logoutOrLoginDate
        }).ToList();
    }
}
    
