using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TibiaCharFinderAPI.Entities;

namespace TibiaCharFinderAPI.Models
{
    public class CreateWorldDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsAvailable { get; set; }
    }
}
