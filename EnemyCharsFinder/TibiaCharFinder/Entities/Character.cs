using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TibiaCharacterFinderAPI.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorldCorrelation> WorldCorrelations { get; set; }
    }
}
