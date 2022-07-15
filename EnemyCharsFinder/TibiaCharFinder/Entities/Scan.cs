using System;

namespace TibiaCharFinderAPI.Entities
{
    public class Scan
    {
        public int Id { get; set; }
        public string CharactersOnline { get; set; }
        public DateTime? ScanCreateDateTime { get; set; }
    }
}
