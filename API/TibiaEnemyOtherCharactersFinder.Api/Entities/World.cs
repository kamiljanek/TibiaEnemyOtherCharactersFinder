namespace TibiaEnemyOtherCharactersFinder.Api.Entities
{
    public class World
    {
        public short WorldId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public bool IsAvailable { get; set; }
        public List<WorldScan> WorldScans { get; set; }
        public List<Character> Characters { get; set; }
        public List<CharacterAction> CharacterLogoutOrLogins { get; set; }
    }
}
