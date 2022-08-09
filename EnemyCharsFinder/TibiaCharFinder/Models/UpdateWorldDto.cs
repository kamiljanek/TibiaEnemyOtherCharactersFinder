using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TibiaCharacterFinderAPI.Models
{
    public class UpdateWorldDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsAvailable { get; set; }
    }
}
