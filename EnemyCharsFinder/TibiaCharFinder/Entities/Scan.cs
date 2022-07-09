using System;

namespace TibiaCharFinder.Entities
{
    public class Scan
    {
        public int Id { get; set; }
        public string CharactersOnline { get; set; }
        public DateTime ScanCreateDateTime { get; set; }
    }
}
