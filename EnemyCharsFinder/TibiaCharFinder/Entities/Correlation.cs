using System;
using System.Collections.Generic;

namespace TibiaCharFinder.Entities
{
    public class Correlation
    {
        public int Id { get; set; }
        public int CharacterId { get; set; }
        public DateTime LogInOrLogOutDateTime { get; set; }
        public string PossibleOtherCharacters { get; set; }
        public virtual Character Character { get; set; }
    }
}
