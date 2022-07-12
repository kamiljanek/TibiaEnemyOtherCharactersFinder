using System;

namespace TibiaCharFinder.Entities
{
    public class WorldScan
    {
        public int Id { get; set; }
        public string CharactersOnline { get; set; }
        public int WorldId { get; set; }
        public DateTime? ScanCreateDateTime { get; set; }
        public virtual World World { get; set; }
    }
}
