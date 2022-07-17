using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinderAPI.Entities;

namespace TibiaCharFinderAPI.Models
{
    public class WorldCorrelationDto
    {
        public int PossibleOtherCharacterId { get; set; }
        public string PossibleOtherCharacterName { get; set; }
    }
}
