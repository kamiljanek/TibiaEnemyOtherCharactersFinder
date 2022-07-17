using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinderAPI.Entities;

namespace TibiaCharFinderAPI.Models
{
    public class WorldScanDto
    {
        public string CharactersOnline { get; set; }
        public string World { get; set; }
        public DateTime ScanCreateDateTime { get; set; }
    }
}
