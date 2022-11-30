using Castle.Core.Internal;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
            if (_dbContext.Database.CanConnect())
            {
                var availableWorlds = await GetAvailableWorldsAsNoTruckingAsync();
                NpgsqlConnection.ClearAllPools();

                foreach (var availableWorld in availableWorlds)
                {
                    var twoWorldScans = GetFirlsTwoWorldScans(availableWorld.WorldId);

                    if (twoWorldScans.Count < 2)
                    {
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
                            SeedCharacterActions(logoutNames, loginNames, twoWorldScans);
                            try
                            {
                                await SeedCharacterCorrelationsAsync();
                                SoftDeleteWorldScan(twoWorldScans[0]);
                            }
                            catch (Exception e)
                            {
                                ClearCharacterActions();
                                Console.WriteLine(e);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            _dbContext.Database.RollbackTransaction();
                        }
                    }
                    await ClearCharacterActionsAsync();
                    Console.WriteLine($"{twoWorldScans[0].WorldScanId} - world_id = {twoWorldScans[0].WorldId}");
                }
            }
            else
            {
                Console.WriteLine("Cannot connect to DB");
            }
        }

        private async Task ClearCharacterActionsAsync()
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                await connection.ExecuteAsync(GenerateQueries.NpgsqlClearCharacterActions);
            }
        }

        private void ClearCharacterActions()
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                connection.Execute(GenerateQueries.NpgsqlClearCharacterActions);
            }
        }

        private async Task SeedCharacterCorrelationsAsync()
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                await connection.ExecuteAsync(GenerateQueries.NpgsqlCreateCharacterIfNotExist);
            }

            NpgsqlConnection.ClearAllPools();

            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                await connection.ExecuteAsync(GenerateQueries.NpgsqlCreateCharacterCorrelation);
            }
        }

        private void SeedCharacterCorrelations()
        {
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                connection.Execute(GenerateQueries.NpgsqlCreateCharacterIfNotExist);
            }
            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                connection.Execute(GenerateQueries.NpgsqlCreateCharacterCorrelation);
            }
        }

        private List<string> GetLoginNames(List<WorldScan> twoWorldScans) => GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToList();

        private List<string> GetLogoutNames(List<WorldScan> twoWorldScans) => GetNames(twoWorldScans[0]).Except(GetNames(twoWorldScans[1])).ToList();

        private void SoftDeleteWorldScan(WorldScan worldScan)
        {
            worldScan.IsDeleted = true;
            _dbContext.SaveChanges();
        }

        private async Task SeedCharacterActionsAsync(List<string> logoutNames, List<string> loginNames, List<WorldScan> twoWorldScans)
        {
            await SeedLogoutCharactersActionsAsync(logoutNames, twoWorldScans[0]); // UNDONE: zoptymalizowac metode na select
            await SeedLoginCharactersActionsAsync(loginNames, twoWorldScans[1]); // UNDONE: zoptymalizowac metode na select
        }

        private void SeedCharacterActions(List<string> logoutNames, List<string> loginNames, List<WorldScan> twoWorldScans)
        {
            SeedLogoutCharactersActions(logoutNames, twoWorldScans[0]); // UNDONE: zoptymalizowac metode na select
            SeedLoginCharactersActions(loginNames, twoWorldScans[1]); // UNDONE: zoptymalizowac metode na select
        }

        private async Task SeedLogoutCharactersActionsAsync(List<string> logoutNames, WorldScan worldScan)
        {
            var logoutCharacters = new List<CharacterAction>();
            foreach (var logoutName in logoutNames)
            {
                var logoutCharacter = CreateCharacterLogoutOrLogin(logoutName, false, worldScan);
                logoutCharacters.Add(logoutCharacter);
            }
            _dbContext.CharacterActions.AddRange(logoutCharacters);
            await _dbContext.SaveChangesAsync();
        }

        private void SeedLogoutCharactersActions(List<string> logoutNames, WorldScan worldScan)
        {
            var logoutCharacters = new List<CharacterAction>();
            foreach (var logoutName in logoutNames)
            {
                var logoutCharacter = CreateCharacterLogoutOrLogin(logoutName, false, worldScan);
                logoutCharacters.Add(logoutCharacter);
            }
            _dbContext.CharacterActions.AddRange(logoutCharacters);
            _dbContext.SaveChanges();
        }

        private async Task SeedLoginCharactersActionsAsync(List<string> loginNames, WorldScan worldScan)
        {
            var loginCharacters = new List<CharacterAction>();
            foreach (var loginName in loginNames)
            {
                var loginCharacter = CreateCharacterLogoutOrLogin(loginName, true, worldScan);
                loginCharacters.Add(loginCharacter);
            }
            _dbContext.CharacterActions.AddRange(loginCharacters);
            await _dbContext.SaveChangesAsync();
        }

        private void SeedLoginCharactersActions(List<string> loginNames, WorldScan worldScan)
        {
            var loginCharacters = new List<CharacterAction>();
            foreach (var loginName in loginNames)
            {
                var loginCharacter = CreateCharacterLogoutOrLogin(loginName, true, worldScan);
                loginCharacters.Add(loginCharacter);
            }
            _dbContext.CharacterActions.AddRange(loginCharacters);
            _dbContext.SaveChanges();
        }

        private CharacterAction CreateCharacterLogoutOrLogin(string characterName, bool isOnline, WorldScan worldScan)
        {
            return new CharacterAction()
            {
                CharacterName = characterName,
                WorldScanId = worldScan.WorldScanId,
                WorldId = worldScan.WorldId,
                IsOnline = isOnline
            };
        }

        private IQueryable<WorldScan> GetFirstTwoWorldScansFromSpecificWorld(short world)
        {
            try
            {
                return _dbContext.WorldScans.Where(w => w.WorldId == world && !w.IsDeleted)
                    .OrderBy(w => w.WorldScanId).Take(2).AsNoTracking();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string> GetNames(WorldScan worldScan)
        {
            var names = worldScan.CharactersOnline.Split("|").ToList();
            names.RemoveAll(string.IsNullOrWhiteSpace);
            return names;
        }
    }
}
