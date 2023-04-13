namespace TibiaEnemyOtherCharactersFinder.Domain.Entities;

public class Character : IEntity
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

    /// <summary>
    /// Boolean wether found name in scan or not
    /// </summary>
    public bool FoundInScan { get; set; }

    // Associations
    public World World { get; set; }
    public List<CharacterCorrelation> LogoutCharacterCorrelations { get; set; }
    public List<CharacterCorrelation> LoginCharacterCorrelations { get; set; }
}
