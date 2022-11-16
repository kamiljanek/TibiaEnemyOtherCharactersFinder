using Castle.Core.Internal;
using Dapper;
using Shared.Database.Queries.Sql;
using Shared.Enums;
using Shared.Providers;
using TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

namespace CharacterAnalyserSeeder
{
    public class CharacterActionSeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private readonly IDapperConnectionProvider _connectionProvider;

        public CharacterActionSeeder(TibiaCharacterFinderDbContext dbContext, IDapperConnectionProvider connectionProvider)
        {
            _dbContext = dbContext;
            _connectionProvider = connectionProvider;
        }

        public void Seed()
        {
            var logoutNames = new List<string>();
            var loginNames = new List<string>();
            var world = GetSpecificWorldIdIfAvailable(EWorldType.Premia);

            if (world == 0)
            {
                return;
            }

            var twoWorldScans = GetFirstTwoWorldScansFromSpecificWorld(world).ToList();

            if (twoWorldScans.Count < 2)
            {
                return;
            }

            logoutNames = GetNames(twoWorldScans[0]).Except(GetNames(twoWorldScans[1])).ToList();

            if (!logoutNames.IsNullOrEmpty())
            {
                loginNames = GetNames(twoWorldScans[1]).Except(GetNames(twoWorldScans[0])).ToList();
            }

            if (!logoutNames.IsNullOrEmpty() && !loginNames.IsNullOrEmpty())
            {
                try
                {
                    _dbContext.Database.BeginTransaction();
                    SeedLogoutCharacters(logoutNames, twoWorldScans[0]);
                    SeedLoginCharacters(loginNames, twoWorldScans[1]);
                    _dbContext.SaveChanges();
                    _dbContext.Database.CommitTransaction();

                    try
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
                        {
                            connection.Execute(GenerateQueries.NpgsqlClearCharacterActions);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _dbContext.Database.RollbackTransaction();
                }
            }

            using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
            {
                connection.Execute(GenerateQueries.NpgsqlClearCharacterActions);
            }

            SoftDeleteWorldScan(twoWorldScans[0]);
            Console.WriteLine(twoWorldScans[0].WorldScanId);
        }

        private void SoftDeleteWorldScan(WorldScan worldScan)
        {
            worldScan.IsDeleted = true;
            _dbContext.SaveChanges();
        }

        private void SeedLogoutCharacters(List<string> logoutNames, WorldScan worldScan)
        {
            var logoutCharacters = new List<CharacterAction>();
            foreach (var logoutName in logoutNames)
            {
                var logoutCharacter = CreateCharacterLogoutOrLogin(logoutName, false, worldScan);
                logoutCharacters.Add(logoutCharacter);
            }
            _dbContext.CharacterActions.AddRange(logoutCharacters);
        }

        private void SeedLoginCharacters(List<string> loginNames, WorldScan worldScan)
        {
            var loginCharacters = new List<CharacterAction>();
            foreach (var loginName in loginNames)
            {
                var loginCharacter = CreateCharacterLogoutOrLogin(loginName, true, worldScan);
                loginCharacters.Add(loginCharacter);
            }
            _dbContext.CharacterActions.AddRange(loginCharacters);
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
                return _dbContext.WorldScans.Where(w => w.WorldId == world && w.IsDeleted == false)
                    .OrderBy(w => w.WorldScanId).Take(2);
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

        private short GetSpecificWorldIdIfAvailable(EWorldType worldType)
        {
            return _dbContext.Worlds.FirstOrDefault(w => w.IsAvailable == true && w.Name == worldType.ToString()).WorldId;
        }
    }
}
