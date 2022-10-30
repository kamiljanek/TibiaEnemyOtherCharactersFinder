using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TibiaEnemyOtherCharactersFinder.Api.Entities
{
    public class Character
    {
        public int CharacterId { get; set; }
        public string Name { get; set; }
        public short WorldId { get; set; }
        public World World { get; set; }
        public List<CharacterCorrelation> LogoutWorldCorrelations { get; set; }
        public List<CharacterCorrelation> LoginWorldCorrelations { get; set; }


    }
}
