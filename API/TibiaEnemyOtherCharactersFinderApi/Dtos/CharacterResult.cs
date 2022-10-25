using TibiaEnemyOtherCharactersFinderApi.Models;

namespace TibiaEnemyOtherCharactersFinderApi.Dtos
{
    public class CharacterResult
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorldCorrelationDto> LogoutWorldCorrelations { get; set; }
        public List<WorldCorrelationDto> LoginWorldCorrelations { get; set; }
    }
}
