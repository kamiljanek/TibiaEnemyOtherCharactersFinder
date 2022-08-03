using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinderAPI.Entities;

namespace TibiaCharacterFinderAPI.Entities
{
    public class OptimizedWorldCorrelation
    {
        public int Id { get; set; }
        public int LogoutOrLoginCharacterId { get; set; }
        public string PossibleOtherCharactersId { get; set; }
        public Character LogoutOrLoginCharacter { get; set; }
    }
}
