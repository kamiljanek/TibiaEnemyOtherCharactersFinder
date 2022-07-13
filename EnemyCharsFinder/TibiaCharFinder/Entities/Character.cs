using System.Collections.Generic;

namespace TibiaCharFinder.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorldCorrelation> WorldCorrelationsLogout { get; set; } = new List<WorldCorrelation>();
        //public List<WorldCorrelation> WorldCorrelationsLogin { get; set; } = new List<WorldCorrelation>();
    }
}
