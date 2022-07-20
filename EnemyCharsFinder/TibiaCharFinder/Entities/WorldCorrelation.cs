using System;

namespace TibiaCharFinderAPI.Entities
{
    public class WorldCorrelation
    {
        public int Id { get; set; }
        public int LogoutCharacterId { get; set; }
        public int LoginCharacterId { get; set; }
        public virtual Character LogoutCharacter { get; set; }
        public virtual Character LoginCharacter { get; set; }

    }
}
