using Castle.Core.Internal;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Shared.Database.Queries.Sql;
using Shared.Providers;
using TibiaEnemyOtherCharactersFinder.Application.Services;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace CharacterAnalyserSeeder
{
    public class CharacterActionSeeder : Model
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly IDapperConnectionProvider _connectionProvider;

        public CharacterActionSeeder(TibiaCharacterFinderDbContext dbContext, IDapperConnectionProvider connectionProvider) : base(dbContext)
        {
            _dbContext = dbContext;
            _connectionProvider = connectionProvider;
        }

        public async Task Seed()
        {
            var availableWorlds = await GetAvailableWorldsAsNoTruckingAsync();

            foreach (var availableWorld in availableWorlds)
            {
                var twoWorldScans = await GetFirstTwoWorldScansAsync(availableWorld.WorldId);

                if (twoWorldScans.Count < 2)
                {
                    continue;
                }

                var timeDifference = twoWorldScans[1].ScanCreateDateTime - twoWorldScans[0].ScanCreateDateTime;
                if (timeDifference.TotalMinutes > 15)
                {
                    await SoftDeleteWorldScanAsync(twoWorldScans[0]);
                    continue;
                }

                var logoutNames = new List<string>();
                var loginNames = new List<string>();

                logoutNames = GetLogoutNames(twoWorldScans);

                if (!logoutNames.IsNullOrEmpty())
                {
                    loginNames = GetLoginNames(twoWorldScans);
                }

                if (!logoutNames.IsNullOrEmpty() && !loginNames.IsNullOrEmpty())
                {
                    try
                    {
                        await SeedCharacterActionsAsync(logoutNames, loginNames, twoWorldScans);
                        try
                        {
                            await SeedCharacterCorrelationsAsync();
                            await SoftDeleteWorldScanAsync(twoWorldScans[0]);
                        }
                        catch (Exception e)
                        {
                            await ClearCharacterActionsAsync();
                            Console.WriteLine(e);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else
                {
                    await SoftDeleteWorldScanAsync(twoWorldScans[0]);
                }
                await ClearCharacterActionsAsync();
                Console.WriteLine($"{twoWorldScans[0].WorldScanId} - world_id = {twoWorldScans[0].WorldId}");
            }
        }

        private async Task ClearCharacterActionsAsync()
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                await connection.ExecuteAsync(GenerateQueries.NpgsqlClearCharacterActions);
            }
        }

        private async Task SeedCharacterCorrelationsAsync()
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                var characterNumber = await connection.ExecuteAsync(GenerateQueries.NpgsqlCreateCharacterIfNotExist);
                var updateNumber = await connection.ExecuteAsync(GenerateQueries.NpgsqlUpdateCharacterCorrelationIfExist);
                var insertNumber = await connection.ExecuteAsync(GenerateQueries.NpgsqlCreateCharacterCorrelationIfNotExist);
            }
        }

        private List<string> GetLoginNames(List<WorldScan> twoWorldScans) => GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToList();

        private List<string> GetLogoutNames(List<WorldScan> twoWorldScans) => GetNames(twoWorldScans[0]).Except(GetNames(twoWorldScans[1])).ToList();

        private async Task SoftDeleteWorldScanAsync(WorldScan worldScan)
        {
            worldScan.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedCharacterActionsAsync(List<string> logoutNames, List<string> loginNames, List<WorldScan> twoWorldScans)
        {
            await SeedLogoutCharactersActionsAsync(logoutNames, twoWorldScans[0]); // UNDONE: zoptymalizowac metode na select
            await SeedLoginCharactersActionsAsync(loginNames, twoWorldScans[1]); // UNDONE: zoptymalizowac metode na select
        }

        private async Task SeedLogoutCharactersActionsAsync(List<string> logoutNames, WorldScan worldScan)
        {
            var logoutCharacters = new List<CharacterAction>();
            foreach (var logoutName in logoutNames)
            {
                var logoutCharacter = CreateCharacterLogoutOrLoginAsync(logoutName, false, worldScan);
                logoutCharacters.Add(await logoutCharacter);
            }
            _dbContext.CharacterActions.AddRange(logoutCharacters);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedLoginCharactersActionsAsync(List<string> loginNames, WorldScan worldScan)
        {
            var loginCharacters = new List<CharacterAction>();
            foreach (var loginName in loginNames)
            {
                var loginCharacter = CreateCharacterLogoutOrLoginAsync(loginName, true, worldScan);
                loginCharacters.Add(await loginCharacter);
            }
            _dbContext.CharacterActions.AddRange(loginCharacters);
            await _dbContext.SaveChangesAsync();
        }

        private async Task<CharacterAction> CreateCharacterLogoutOrLoginAsync(string characterName, bool isOnline, WorldScan worldScan)
        {
            var characterAction = Task.FromResult(new CharacterAction()
            {
                CharacterName = characterName,
                WorldScanId = worldScan.WorldScanId,
                WorldId = worldScan.WorldId,
                IsOnline = isOnline
            });
            return await characterAction;
        }

        private List<string> GetNames(WorldScan worldScan)
        {
            var names = worldScan.CharactersOnline.Split("|").ToList();
            names.RemoveAll(string.IsNullOrWhiteSpace);
            return names;
        }
    }
}
