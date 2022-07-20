using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TibiaCharFinderAPI.Entities
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<WorldCorrelation> LoginWorldCorrelations { get; set; }
        public virtual List<WorldCorrelation> LogoutWorldCorrelations { get; set; }

    }
}
