namespace TibiaCharacterFinderAPI.Models
{
    public class CharacterDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorldCorrelationDto> LogoutWorldCorrelations { get; set; }
        public List<WorldCorrelationDto> LoginWorldCorrelations { get; set; }
    }
}
