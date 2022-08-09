using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCharacterFinderAPI.Entities
{
    public class WorldCorrelation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public string PossibleOtherCharactersId { get; set; }
        public Character Character { get; set; }
    }
}
