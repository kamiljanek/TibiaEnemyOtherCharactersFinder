using System.Collections.Generic;

namespace TibiaCharFinderAPI.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorldCorrelation> WorldCorrelationsLogout { get; set; } = new List<WorldCorrelation>();
    }
}
