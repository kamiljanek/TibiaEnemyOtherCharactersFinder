using System;

namespace TibiaCharFinder.Entities
{
    public class WorldCorrelation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public DateTime? LogInOrLogOutDateTime { get; set; }
        public int PossibleCharacterId { get; set; }
        public virtual Character Character { get; set; }
    }
}
