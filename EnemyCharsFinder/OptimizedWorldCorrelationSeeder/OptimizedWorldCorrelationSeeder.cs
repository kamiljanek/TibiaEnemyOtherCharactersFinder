using System.Text;
using TibiaCharacterFinderAPI.Entities;
using TibiaCharFinderAPI.Entities;
using TibiaCharFinderAPI.Models;

namespace OptimizedWorldCorrelationSeeder
{
    public class OptimizedWorldCorrelationSeeder : Model, ISeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;

        public OptimizedWorldCorrelationSeeder(TibiaCharacterFinderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {

                var worlds = GetAvailableWorlds();

                _dbContext.Database.BeginTransaction();

                try
                {
                    foreach (var world in worlds)
                    {
                        var worldScans = GetWorldScansFromSpecificServer(world);
                        while (worldScans.Count > 1)
                        {
                            var namesThatLogged_Out = GetLogout_Names(worldScans);
                            SeedCharacters(namesThatLogged_Out);
                            var charactersThatLogged_Out = GetCharactersBasedOnNames(namesThatLogged_Out);

                            var namesThatLogged_In = GetLogin_Names(worldScans);
                            SeedCharacters(namesThatLogged_In);
                            var charactersThatLogged_In = GetCharactersBasedOnNames(namesThatLogged_In);

                            foreach (var loggedOutCharacter in charactersThatLogged_Out)
                            {
                                var worldCorrelation = CreateOptimizedWorldCorrelation(loggedOutCharacter, charactersThatLogged_In);
                                _dbContext.OptimizedWorldCorrelations.Add(worldCorrelation);
                            }

                            foreach (var loggedInCharacter in charactersThatLogged_In)
                            {
                                var worldCorrelation = CreateOptimizedWorldCorrelation(loggedInCharacter, charactersThatLogged_Out);
                                _dbContext.OptimizedWorldCorrelations.Add(worldCorrelation);
                            }


                            worldScans.Remove(worldScans[0]);
                        }
                    }
                    _dbContext.SaveChanges();
                    _dbContext.Database.CommitTransaction();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _dbContext.Database.RollbackTransaction();
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
        public OptimizedWorldCorrelation CreateOptimizedWorldCorrelation(Character character, List<Character> possibleOtherCharacters)
        {
            var stringBuilder = new StringBuilder();

            foreach (var possibleOtherCharacter in possibleOtherCharacters)
            {
                stringBuilder.Append($"{possibleOtherCharacter.Id}|");
            }

            return new OptimizedWorldCorrelation
            {
                LogoutOrLoginCharacterId = character.Id,
                PossibleOtherCharactersId = stringBuilder.ToString(),
            };
        }

        public void SeedCharacters(List<string> charactersName)
        {
            foreach (var characterName in charactersName)
            {
                if (!_dbContext.Characters.Any(x => x.Name == characterName))
                {
                    var character = CreateCharacter(characterName);
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
            var firstOnlineNames = GetFirstOnlineNames(worldScans);
            var nextOnlineNames = GetNextOnlineNames(worldScans);
            return firstOnlineNames.Except(nextOnlineNames).ToList();
        }
        private List<string> GetLogin_Names(List<WorldScan> worldScans)
        {
            var firstOnlineNames = GetFirstOnlineNames(worldScans);
            var nextOnlineNames = GetNextOnlineNames(worldScans);
            return nextOnlineNames.Except(firstOnlineNames).ToList();
        }
        private List<string> GetFirstOnlineNames(List<WorldScan> worldScans)
        {
            return GetOnlineNames(worldScans, 0);
        }
        private List<string> GetNextOnlineNames(List<WorldScan> worldScans)
        {
            return GetOnlineNames(worldScans, 1);
        }
        private List<string> GetOnlineNames(List<WorldScan> worldScans, int index)
        {
            return worldScans[index].CharactersOnline.Split("|").ToList();
        }
    }
}
