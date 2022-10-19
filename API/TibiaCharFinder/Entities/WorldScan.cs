using System;

namespace TibiaCharacterFinderAPI.Entities
{
    public class WorldScan
    {
        public int WorldScanId { get; set; }
        public string CharactersOnline { get; set; }
        public short WorldId { get; set; }
        public World World { get; set; }
        public DateTime ScanCreateDateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
