using TibiaCharacterFinderAPI.Entities;
using TibiaCharacterFinderAPI.Models;

namespace CharacterLogoutOrLoginSeeder
{
    public class CharacterLogoutOrLoginSeeder : Model, ISeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;

        public CharacterLogoutOrLoginSeeder(TibiaCharacterFinderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {

            var firstScanNames = new List<string>();
            var nextScanNames = new List<string>();
            var logoutNames = new List<string>();
            var loginNames = new List<string>();

            if (_dbContext.Database.CanConnect())
            {
                var worlds = GetAvailableWorlds();
                var premia = worlds.FirstOrDefault(x => x.Name == "Premia");
                var worldScansFromSpecificServer = GetWorldScansFromSpecificServer(premia);
                for (int b = 0; b < worldScansFromSpecificServer.Count - 1; b++)
                {
                    firstScanNames = GetNamesFromSpecificScan(worldScansFromSpecificServer[b]);
                    nextScanNames = GetNamesFromSpecificScan(worldScansFromSpecificServer[b + 1]);
                    logoutNames = firstScanNames.Except(nextScanNames).ToList();
                    loginNames = nextScanNames.Except(firstScanNames).ToList();
                    var logoutAndLoginNames = new List<string>();
                    logoutAndLoginNames.AddRange(loginNames);
                    logoutAndLoginNames.AddRange(logoutNames);
                    try
                    {
                        _dbContext.Database.BeginTransaction();
                        SeedCharacters(logoutAndLoginNames, premia);
                        _dbContext.SaveChanges();
                        _dbContext.Database.CommitTransaction();
                        Console.WriteLine($"Create Characters - World = {premia.Name}, World Scan ID= {b}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        _dbContext.Database.RollbackTransaction();
                    }

                    try
                    {
                        _dbContext.Database.BeginTransaction();
                        SeedLogoutCharacters(logoutNames, worldScansFromSpecificServer[b]);
                        SeedLoginCharacters(loginNames, worldScansFromSpecificServer[b+1]);
                        _dbContext.SaveChanges();
                        _dbContext.Database.CommitTransaction();
                        Console.WriteLine($"Create LogoutOrLogin - World = {premia.Name}, World Scan ID= {b}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        _dbContext.Database.RollbackTransaction();
                    }
                }
            }
        }

        private void SeedLoginCharacters(List<string> loginNames, WorldScan worldScan)
        {
            foreach (var loginName in loginNames)
            {
                var character = _dbContext.Characters.FirstOrDefault(x => x.Name == loginName);
                var loginCharacter = CreateLoginCharacter(character, worldScan);
                _dbContext.CharacterLogoutOrLogins.Add(loginCharacter);

            }
        }
        private CharacterLogoutOrLogin CreateLoginCharacter(Character character, WorldScan worldScan)
        {
            return new CharacterLogoutOrLogin()
            {
                CharacterId = character.CharacterId,
                WorldScanId = worldScan.WorldScanId,
                WorldId = worldScan.WorldId,
                IsOnline = true
                
            };
        }

        private void SeedLogoutCharacters(List<string> logoutNames, WorldScan worldScan)
        {
            foreach (var logoutName in logoutNames)
            {
                var character = _dbContext.Characters.FirstOrDefault(x => x.Name == logoutName);
                var logoutCharacter = CreateLogoutCharacter(character, worldScan);
                _dbContext.CharacterLogoutOrLogins.Add(logoutCharacter);

            }
        }

        private CharacterLogoutOrLogin CreateLogoutCharacter(Character character, WorldScan worldScan)
        {
            return new CharacterLogoutOrLogin()
            {
                CharacterId = character.CharacterId,
                WorldScanId = worldScan.WorldScanId,
                WorldId = worldScan.WorldId,
                IsOnline = false
            };
        }

        private void SeedCharacters(List<string> names, World world)
        {
            foreach (var name in names)
            {
                if (!_dbContext.Characters.Any(x => x.Name == name))
                {
                    var character = CreateCharacter(name, world);
                    _dbContext.Characters.Add(character);
                }
            }
        }

        private Character CreateCharacter(string name, World world)
        {
            return new Character()
            {
                Name = name,
                WorldId = world.WorldId
            };
        }
        private List<WorldScan> GetWorldScansFromSpecificServer(World world)
        {
            return _dbContext.WorldScans.Where(w => w.World == world).ToList();
        }

        private List<string> GetLogout_Names(List<WorldScan> worldScans)
        {
            var firstOnlineNames = GetNamesFromFirstScan(worldScans);
            var nextOnlineNames = GetNamesFromNextScan(worldScans);
            return firstOnlineNames.Except(nextOnlineNames).ToList();
        }
        private List<string> GetLogin_Names(List<WorldScan> worldScans)
        {
            var firstOnlineNames = GetNamesFromFirstScan(worldScans);
            var nextOnlineNames = GetNamesFromNextScan(worldScans);
            return nextOnlineNames.Except(firstOnlineNames).ToList();
        }
        private List<string> GetNamesFromFirstScan(List<WorldScan> worldScans)
        {
            return GetNamesFromScan(worldScans, 0);
        }
        private List<string> GetNamesFromNextScan(List<WorldScan> worldScans)
        {
            return GetNamesFromScan(worldScans, 1);
        }
        private List<string> GetNamesFromScan(List<WorldScan> worldScans, int index)
        {
            return worldScans[index].CharactersOnline.Split("|").ToList();
        }
        private List<string> GetNamesFromSpecificScan(WorldScan worldScan)
        {
            return worldScan.CharactersOnline.Split("|").ToList();
        }
    }
}
