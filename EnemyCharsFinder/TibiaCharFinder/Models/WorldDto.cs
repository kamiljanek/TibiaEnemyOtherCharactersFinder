namespace TibiaCharacterFinderAPI.Models
{
    public class WorldDto
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public List<WorldScanDto> WorldScans { get; set; }
    }
}
