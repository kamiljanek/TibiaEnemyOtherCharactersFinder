using Castle.Core.Internal;
using System.Text;
using TibiaCharacterFinderAPI.Entities;
using TibiaCharacterFinderAPI.Models;

namespace WorldCorrelationSeeder
{
    public class WorldCorrelationSeeder : Model, ISeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;

        public WorldCorrelationSeeder(TibiaCharacterFinderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                var worlds = GetAvailableWorlds();

                for (int i = 29; i < worlds.Count; i++)
                {
                    var characterLogoutOrLoginIds = GetCharacterLogoutOrLoginIds(worlds[i]).OrderBy(x=>x.WorldScanId).ToList();
                    while (!characterLogoutOrLoginIds.IsNullOrEmpty())
                    {
                        try
                        {
                            _dbContext.Database.BeginTransaction();
                            var firstLogoutCharacters = GetFirstLogoutCharacters(characterLogoutOrLoginIds);
                            var firstLoginCharacters = GetFirstLoginCharacters(characterLogoutOrLoginIds);
                            foreach (var characterLogout in firstLogoutCharacters)
                            {
                                foreach (var characterLogin in firstLoginCharacters)
                                {
                                    SeedCharacterCorrelation(characterLogout, characterLogin);
                                }
                            }
                            DeleteRecords(firstLogoutCharacters);
                            DeleteRecords(firstLoginCharacters);
                            _dbContext.SaveChanges();
                            _dbContext.Database.CommitTransaction();
                            characterLogoutOrLoginIds = GetCharacterLogoutOrLoginIds(worlds[i]).OrderBy(x => x.WorldScanId).ToList();
                            Console.WriteLine(characterLogoutOrLoginIds.Count);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            _dbContext.Database.RollbackTransaction();
                        }
                    }

                }
            }
        }

        private void DeleteRecords(List<CharacterLogoutOrLogin> firstLogCharacters)
        {
            foreach (var logCharacter in firstLogCharacters)
            {
                _dbContext.CharacterLogoutOrLogins.Remove(logCharacter);
            }
            //_dbContext.SaveChanges();
        }

        private void SeedCharacterCorrelation(CharacterLogoutOrLogin characterLogout, CharacterLogoutOrLogin characterLogin)
        {
            var characterCorrelation = _dbContext.CharacterCorrelations.Where(x => x.LogoutCharacterId == 7)
                .FirstOrDefault(x => x.LoginCharacterId == 3);
            if (characterCorrelation == null)
            {
                var nextCharacterCorrelation = _dbContext.CharacterCorrelations.Where(x => x.LogoutCharacterId == 3)
                    .FirstOrDefault(x => x.LoginCharacterId == 5);
                if (nextCharacterCorrelation != null)
                {
                    AutoIncreaseNumberOfMatches(nextCharacterCorrelation);
                }
                else
                {
                    var newCharacterCorrelation = CreateCharacterCorrelation(characterLogout, characterLogin);
                    _dbContext.CharacterCorrelations.Add(newCharacterCorrelation);
                }

            }
            else
            {
                AutoIncreaseNumberOfMatches(characterCorrelation);
            }

        }

        private void AutoIncreaseNumberOfMatches(CharacterCorrelation characterCorrelation)
        {
            characterCorrelation.NumberOfMatches += 1;
            //_dbContext.SaveChanges();
        }

        private CharacterCorrelation CreateCharacterCorrelation(CharacterLogoutOrLogin characterLogout, CharacterLogoutOrLogin characterLogin)
        {
            return new CharacterCorrelation()
            {
                LogoutCharacterId = 1,
                LoginCharacterId = 2,
                NumberOfMatches = 1
            };
        }

        private List<CharacterLogoutOrLogin> GetFirstLogoutCharacters(List<CharacterLogoutOrLogin> characterLogoutOrLoginIds)
        {
            var worldScanId = characterLogoutOrLoginIds.Where(y => y.IsOnline == false).FirstOrDefault().WorldScanId;
            return characterLogoutOrLoginIds.Where(y => y.IsOnline == false).Where(x => x.WorldScanId == worldScanId).ToList();
        }
        private List<CharacterLogoutOrLogin> GetFirstLoginCharacters(List<CharacterLogoutOrLogin> characterLogoutOrLoginIds)
        {
            var worldScanId = characterLogoutOrLoginIds.Where(y => y.IsOnline == true).FirstOrDefault().WorldScanId;
            return characterLogoutOrLoginIds.Where(y => y.IsOnline == true).Where(x => x.WorldScanId == worldScanId).ToList();
        }

        private List<CharacterLogoutOrLogin> GetCharacterLogoutOrLoginIds(World world)
        {
            return _dbContext.CharacterLogoutOrLogins.Where(x => x.WorldId == world.WorldId).ToList();
        }

        //public List<Character> GetCharactersBasedOnNames(List<string> charactersName)
        //{
        //    var listOfCharacters = new List<Character>();
        //    foreach (var character in charactersName)
        //    {
        //        var foundCharacter = _dbContext.Characters.FirstOrDefault(c => c.Name == character);
        //        listOfCharacters.Add(foundCharacter);
        //    }
        //    return listOfCharacters;
        //}
        //public CharacterCorrelation CreateWorldCorrelation(Character character, List<Character> possibleOtherCharacters)
        //{
        //    var stringBuilder = new StringBuilder();

        //    foreach (var possibleOtherCharacter in possibleOtherCharacters)
        //    {
        //        stringBuilder.Append($"{possibleOtherCharacter.CharacterId}|");
        //    }

        //    return new CharacterCorrelation
        //    {
        //        LogoutCharacterId = character.CharacterId,
        //        //PossibleOtherCharacterId = stringBuilder.ToString(),
        //    };
        //}

        //public void SeedCharacters(List<string> charactersName)
        //{
        //    foreach (var characterName in charactersName)
        //    {
        //        if (!_dbContext.Characters.Any(x => x.Name == characterName))
        //        {
        //            var character = CreateCharacter(characterName);
        //            _dbContext.Characters.Add(character);
        //        }
        //    }
        //    _dbContext.SaveChanges();
        //}

        //public Character CreateCharacter(string characterName)
        //{
        //    return new Character { Name = characterName };
        //}
        //private List<WorldScan> GetWorldScansFromSpecificServer(World world)
        //{
        //    return _dbContext.WorldScans.Where(w => w.World == world).ToList();
        //}
        //private List<string> GetLogout_Names(List<WorldScan> worldScans)
        //{
        //    var firstOnlineNames = GetFirstOnlineNames(worldScans);
        //    var nextOnlineNames = GetNextOnlineNames(worldScans);
        //    return firstOnlineNames.Except(nextOnlineNames).ToList();
        //}
        //private List<string> GetLogin_Names(List<WorldScan> worldScans)
        //{
        //    var firstOnlineNames = GetFirstOnlineNames(worldScans);
        //    var nextOnlineNames = GetNextOnlineNames(worldScans);
        //    return nextOnlineNames.Except(firstOnlineNames).ToList();
        //}
        //private List<string> GetFirstOnlineNames(List<WorldScan> worldScans)
        //{
        //    return GetOnlineNames(worldScans, 0);
        //}
        //private List<string> GetNextOnlineNames(List<WorldScan> worldScans)
        //{
        //    return GetOnlineNames(worldScans, 1);
        //}
        //private List<string> GetOnlineNames(List<WorldScan> worldScans, int index)
        //{
        //    return worldScans[index].CharactersOnline.Split("|").ToList();
        //}
    }
}
