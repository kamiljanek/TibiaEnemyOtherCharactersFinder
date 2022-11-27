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

        public void Seed()
        {
            var availableWorlds = GetAvailableWorldsIncludingScans();

            foreach (var availableWorld in availableWorlds)
            {
                var twoWorldScans = availableWorld.WorldScans.Where(scan => !scan.IsDeleted).ToList();

                while (twoWorldScans.Count > 1)
                {

                    var logoutNames = new List<string>();
                    var loginNames = new List<string>();
                    twoWorldScans = availableWorld.WorldScans.Where(scan => !scan.IsDeleted).Take(2).ToList();

                    if (twoWorldScans.Count < 2)
                    {
                        continue;
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
                                    connection.Execute(GenerateQueries.NpgsqlCreateCharacterCorrelation);
                                }
                                //using (var connection = _connectionProvider.GetConnection(EModuleType.PostgreSql))
                                //{
                                //}
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
            }
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
