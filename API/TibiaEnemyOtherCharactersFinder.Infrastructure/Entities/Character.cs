namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

public class Character
{
    /// <summary>
    /// Id of specific character
    /// </summary>
    public int CharacterId { get; set; }

    /// <summary>
    /// Character name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Id of specific world
    /// </summary>
    public short WorldId { get; set; }

    // Associations
    public World World { get; set; }
    public List<CharacterCorrelation> LogoutWorldCorrelations { get; set; }
    public List<CharacterCorrelation> LoginWorldCorrelations { get; set; }
}
