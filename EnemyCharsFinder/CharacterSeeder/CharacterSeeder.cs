using TibiaCharacterFinderAPI.Entities;
using TibiaCharacterFinderAPI.Models;

namespace CharacterSeeder
{
    public class CharacterSeeder : Model, ISeeder
    {
        private readonly TibiaCharacterFinderDbContext _dbContext;

        public CharacterSeeder(TibiaCharacterFinderDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                _dbContext.Database.BeginTransaction();
                try
                {
                    //foreach (var worldScan in _dbContext.WorldScans)
                    //{
                    //    CharacterSeed(worldScan);
                    //}

                    for (int i = 0; i < _dbContext.WorldScans.Count(); i++)
                    {
                        CharacterSeed(_dbContext.WorldScans.Skip(i).FirstOrDefault());
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

        private void CharacterSeed(WorldScan worldScan)
        {
            var names = worldScan.CharactersOnline.Split("|").ToList();

            for (int a = 0; a < names.Count; a++)
            {
                if (!_dbContext.Characters.Any(x => x.Name == names[a]))
                {
                    var character = new Character() { Name = names[a] };
                    _dbContext.Characters.Add(character);
                }
            }
            //foreach (var name in names)
            //{
            //    if (!_dbContext.Characters.Any(x => x.Name == name))
            //    {
            //        var character = new Character() { Name = name };
            //        _dbContext.Characters.Add(character);
            //    }
            //}
        }
    }
}
