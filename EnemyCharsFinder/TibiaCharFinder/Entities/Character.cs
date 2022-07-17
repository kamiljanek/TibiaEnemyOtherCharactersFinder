using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TibiaCharFinderAPI.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("CharacterId")]
        public List<WorldCorrelation> WorldCorrelations { get; set; }
    }
}
