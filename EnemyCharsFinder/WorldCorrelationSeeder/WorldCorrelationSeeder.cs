using TibiaCharFinder.Entities;
using TibiaCharFinder.Models;

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
                var worlds = GetWorldsFromDb();
                foreach (var world in worlds)
                {
                    var worldScans = GetWorldScansFromSpecificServer(world);
                    while (worldScans.Count > 0)
                    {

                        var nameThatHave_Logout = GetLogout_Names(worldScans);
                        SeedCharacters(nameThatHave_Logout);
                        var charactersThatHave_Logout = GetCharactersBasedOnNames(nameThatHave_Logout);

                        var nameThatHave_Login = GetLogin_Names(worldScans);
                        SeedCharacters(nameThatHave_Login);
                        var charactersThatHave_Login = GetCharactersBasedOnNames(nameThatHave_Login);
                        foreach (var character_Logout in charactersThatHave_Logout)
                        {
                            foreach (var character_Login in charactersThatHave_Login)
                            {
                                var worldCorrelation = CreateWorldCorrelation(character_Logout, character_Login);
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
        public WorldCorrelation CreateWorldCorrelation(Character character, Character possibleOtherCharacter)
        {
            return new WorldCorrelation
            {
                CharacterId = character.Id,
                PossibleOtherCharacterId = possibleOtherCharacter.Id,
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
            if (worldScans.Count > 1)
            {
                return worldScans[1].CharactersOnline.Split("\r\n").ToList();
            }
            return worldScans[0].CharactersOnline.Split("\r\n").ToList();
        }
    }
}
