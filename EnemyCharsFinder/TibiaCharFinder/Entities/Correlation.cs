using System;

namespace TibiaCharFinder.Entities
{
    public class Correlation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public int PossibleCharacterId { get; set; }
        public DateTime? LogInOrLogOutDateTime { get; set; }
        public virtual Character Character { get; set; }
    }
}
