using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinderAPI.Entities;

namespace TibiaCharFinderAPI.Models
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorldCorrelationDto> LogoutWorldCorrelations { get; set; }
        public List<WorldCorrelationDto> LoginWorldCorrelations { get; set; }
    }
}
