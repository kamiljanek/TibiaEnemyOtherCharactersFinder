namespace TibiaCharacterFinderAPI.Entities
{
    public class World
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsAvailable { get; set; }
        public List<WorldScan> WorldScans { get; set; }

    }
}
