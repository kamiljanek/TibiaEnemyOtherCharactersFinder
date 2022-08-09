using System;

namespace TibiaCharacterFinderAPI.Entities
{
    public class WorldScan
    {
        public int Id { get; set; }
        public string CharactersOnline { get; set; }
        public int WorldId { get; set; }
        public World World { get; set; }
        public DateTime? ScanCreateDateTime { get; set; }
    }
}
