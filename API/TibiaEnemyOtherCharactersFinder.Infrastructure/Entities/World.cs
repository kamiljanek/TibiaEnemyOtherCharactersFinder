namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

public class World
{
    /// <summary>
    /// Id of world
    /// </summary>
    public short WorldId { get; set; }

    /// <summary>
    /// World name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// World url
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    /// Boolean (true if world is available, otherwise false)
    /// </summary>
    public bool IsAvailable { get; set; }

    // Associations
    public List<WorldScan> WorldScans { get; set; }
    public List<Character> Characters { get; set; }
    public List<CharacterAction> CharacterLogoutOrLogins { get; set; }
}
