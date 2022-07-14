using System;

namespace TibiaCharFinder.Entities
{
    public class WorldCorrelation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public Character Character { get; set; }
        public int PossibleOtherCharacterId { get; set; }
    }
}
