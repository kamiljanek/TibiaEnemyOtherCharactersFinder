using Castle.Core.Internal;
using Dapper;
using Shared.Database.Queries.Sql;
using Shered.Enums;
using TibiaEnemyOtherCharactersFinderApi.Entities;
using TibiaEnemyOtherCharactersFinderApi.Models;
using TibiaEnemyOtherCharactersFinderApi.Providers;

namespace CharacterLogoutOrLoginSeeder
{
    public class CharacterLogoutOrLoginSeeder : Model, ISeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;
        private List<string> firstScanNames;
        private List<string> secondScanNames;
        private List<string> logoutNames;
        private List<string> loginNames;
        private readonly IDapperConnectionProvider _connectionProvider;

        public CharacterLogoutOrLoginSeeder(TibiaCharacterFinderDbContext dbContext, IDapperConnectionProvider dapperConnectionProvider) : base(dbContext)
        {
            _dbContext = dbContext;
            _connectionProvider = dapperConnectionProvider;
        }

        public void Seed()
        {
            var worlds = GetAvailableWorlds();
            var choosenWorld = worlds.FirstOrDefault(x => x.Name == WorldType.Premia.ToString());

            if (choosenWorld == null)
            {
                return;
            }

            var twoWorldScans = GetFirstTwoWorldScansFromSpecificWorld(choosenWorld);

            if (twoWorldScans == null)
            {
                return;
            }

            firstScanNames = GetNames(twoWorldScans[0]);
            secondScanNames = GetNames(twoWorldScans[1]);
            logoutNames = firstScanNames.Except(secondScanNames).ToList();

            if (!logoutNames.IsNullOrEmpty())
            {
                loginNames = secondScanNames.Except(firstScanNames).ToList();
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
                        using (var connection = _connectionProvider.GetConnection(EModuleType.TibiaDB))
                        {
                            connection.Execute(GenerateQueries.CreateCharacterIfNotExist);
                        }
                        using (var connection = _connectionProvider.GetConnection(EModuleType.TibiaDB))
                        {
                            connection.Execute(GenerateQueries.CreateCharacterCorrelation);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        using (var connection = _connectionProvider.GetConnection(EModuleType.TibiaDB))
                        {
                            connection.Execute(GenerateQueries.ClearCharacterLogoutOrLogins);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _dbContext.Database.RollbackTransaction();
                }
            }

            using (var connection = _connectionProvider.GetConnection(EModuleType.TibiaDB))
            {
                connection.Execute(GenerateQueries.ClearCharacterLogoutOrLogins);
            }

            SoftDeleteWorldScan(twoWorldScans[0]);
            Console.WriteLine(twoWorldScans[0].WorldScanId);
        }

        private void SoftDeleteWorldScan(WorldScan worldScan)
        {
            worldScan.IsDeleted = true;
            _dbContext.SaveChanges();
        }

        private void SeedLogoutCharacters(List<string> loginNames, WorldScan worldScan)
        {
            foreach (var loginName in loginNames)
            {
                var loginCharacter = CreateCharacterLogoutOrLogin(loginName, false, worldScan);
                _dbContext.CharacterLogoutOrLogins.Add(loginCharacter);

            }
        }

        private void SeedLoginCharacters(List<string> loginNames, WorldScan worldScan)
        {
            foreach (var loginName in loginNames)
            {
                var loginCharacter = CreateCharacterLogoutOrLogin(loginName, true, worldScan);
                _dbContext.CharacterLogoutOrLogins.Add(loginCharacter);

            }
        }

        private CharacterLogoutOrLogin CreateCharacterLogoutOrLogin(string characterName, bool isOnline, WorldScan worldScan)
        {
            return new CharacterLogoutOrLogin()
            {
                CharacterName = characterName,
                WorldScanId = worldScan.WorldScanId,
                WorldId = worldScan.WorldId,
                IsOnline = isOnline
            };
        }

        private List<WorldScan> GetFirstTwoWorldScansFromSpecificWorld(World world)
        {
            var firstTwoWorldScans = new List<WorldScan>();
            var scanListOrderById = _dbContext.WorldScans.Where(w => w.WorldId == world.WorldId && w.IsDeleted == false).OrderBy(w => w.WorldScanId);
            var firstScan = scanListOrderById.FirstOrDefault();
            var secondScan = scanListOrderById.Skip(1).FirstOrDefault();

            if (firstScan != null && secondScan != null)
            {
                firstTwoWorldScans.Add(firstScan);
                firstTwoWorldScans.Add(secondScan);
                return firstTwoWorldScans;
            }
            return null;
        }

        private List<string> GetNames(WorldScan worldScan)
        {
            var names = worldScan.CharactersOnline.Split("|").ToList();
            names.RemoveAll(string.IsNullOrWhiteSpace);
            return names;
        }
    }
}
