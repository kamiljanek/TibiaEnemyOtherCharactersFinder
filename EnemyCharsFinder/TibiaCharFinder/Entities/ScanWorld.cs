using System;
using System.Collections.Generic;

namespace TibiaCharFinder.Entities
{
    public class ScanWorld
    {
        public int Id { get; set; }
        public string CharactersOnline { get; set; }
        public int WorldId { get; set; }
        public DateTime ScanCreateDateTime { get; set; }
        public virtual World World { get; set; }
    }
}
