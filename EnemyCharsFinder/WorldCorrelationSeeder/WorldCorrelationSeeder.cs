using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace WorldCorrelationSeeder
{
    public class WorldCorrelationSeeder : Model, ISeeder
    {
        private readonly EnemyCharFinderDbContext _dbContext;

        public WorldCorrelationSeeder(EnemyCharFinderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var worlds = GetWorldsFromDbIfIsAvailable();
                foreach (var world in worlds)
                {
                    var worldScans = GetWorldScansFromSpecificServer(world);
                    while (worldScans.Count > 1)
                    {
                        var logoutNames = GetLogout_Names(worldScans);
                        SeedCharacters(logoutNames);
                        var logoutCharacters = GetCharactersBasedOnNames(logoutNames);

                        var loginNames = GetLogin_Names(worldScans);
                        SeedCharacters(loginNames);
                        var loginCharacters = GetCharactersBasedOnNames(loginNames);
                      
                        foreach (var logoutCharacter in logoutCharacters)
                        {
                            foreach (var loginCharacter in loginCharacters)
                            {
                                var worldCorrelation = CreateWorldCorrelation(logoutCharacter, loginCharacter);
                                _dbContext.WorldCorrelations.Add(worldCorrelation);
                            }
                        }
                        worldScans.Remove(worldScans[0]);
                    }
                    _dbContext.SaveChanges();
                }
            }
        }

        public List<Character> GetCharactersBasedOnNames(List<string> charactersName)
        {
            var listOfCharacters = new List<Character>();
            foreach (var character in charactersName)
            {
                var foundCharacter = _dbContext.Characters.FirstOrDefault(c => c.Name == character);
                listOfCharacters.Add(foundCharacter);
            }
            return listOfCharacters;
        }
        public WorldCorrelation CreateWorldCorrelation(Character logoutCharacter, Character loginCharacter)
        {
            return new WorldCorrelation
            {
                LogoutCharacterId = logoutCharacter.Id,
                LogoutCharacter = logoutCharacter,
                LoginCharacterId = loginCharacter.Id,
                LoginCharacter = loginCharacter
            };
        }

        public void SeedCharacters(List<string> charactersName)
        {
            foreach (var characterName in charactersName)
            {
                var character = CreateCharacter(characterName);
                if (!_dbContext.Characters.Any(x => x.Name == character.Name))
                {
                    _dbContext.Characters.Add(character);
                }
            }
            _dbContext.SaveChanges();
        }

        public Character CreateCharacter(string characterName)
        {
            return new Character { Name = characterName };
        }
        private List<WorldScan> GetWorldScansFromSpecificServer(World world)
        {
            return _dbContext.WorldScans.Where(w => w.World == world).ToList();
        }
        private List<string> GetLogout_Names(List<WorldScan> worldScans)
        {
            var firstOnlineNames = GetFirstNamesOnline(worldScans);
            var nextOnlineNames = GetNextNamesOnline(worldScans);
            return firstOnlineNames.Except(nextOnlineNames).ToList();
        }
        private List<string> GetLogin_Names(List<WorldScan> worldScans)
        {
            var firstOnlineNames = GetFirstNamesOnline(worldScans);
            var nextOnlineNames = GetNextNamesOnline(worldScans);
            return nextOnlineNames.Except(firstOnlineNames).ToList();
        }
        private List<string> GetFirstNamesOnline(List<WorldScan> worldScans)
        {
            return worldScans[0].CharactersOnline.Split("\r\n").ToList();
        }
        private List<string> GetNextNamesOnline(List<WorldScan> worldScans)
        {
                return worldScans[1].CharactersOnline.Split("\r\n").ToList();
        }
    }
}
