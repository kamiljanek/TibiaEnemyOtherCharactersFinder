using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TibiaCharacterFinderAPI.Entities;

namespace TibiaCharFinderAPI.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<WorldCorrelation> LoginWorldCorrelations { get; set; }
        public virtual List<WorldCorrelation> LogoutWorldCorrelations { get; set; }
        public virtual List<OptimizedWorldCorrelation> OptimizedWorldCorrelations { get; set; }


    }
}
