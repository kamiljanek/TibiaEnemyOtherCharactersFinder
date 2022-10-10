using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCharacterFinderAPI.Entities
{
    public class CharacterCorrelation
    {
        public int CorrelationId { get; set; }
        public int LogoutCharacterId { get; set; }
        public Character LogoutCharacter { get; set; }
        public int LoginCharacterId { get; set; }
        public Character LoginCharacter { get; set; }
        public short NumberOfMatches { get; set; }
    }
}
