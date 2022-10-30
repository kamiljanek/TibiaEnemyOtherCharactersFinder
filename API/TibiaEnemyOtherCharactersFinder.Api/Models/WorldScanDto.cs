namespace TibiaEnemyOtherCharactersFinder.Api.Models
{
    public class WorldScanDto
    {
        public string CharactersOnline { get; set; }
        public string World { get; set; }
        public DateTime ScanCreateDateTime { get; set; }
    }
}
