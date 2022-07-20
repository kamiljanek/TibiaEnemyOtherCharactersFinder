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
        public int LogoutCharacterId { get; set; }
        public string LogoutCharacterName { get; set; }
        public int LoginCharacterId { get; set; }
        public string LoginCharacterName { get; set; }
    }
}
