namespace TibiaEnemyOtherCharactersFinder.Api.Models
{
    public class WorldCorrelationDto
    {
        public int LogoutCharacterId { get; set; }
        public string LogoutCharacterName { get; set; }
        public int LoginCharacterId { get; set; }
        public string LoginCharacterName { get; set; }
    }
}
