namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

public class CharacterAction
{

    /// <summary>
    /// Id of Character Action record
    /// </summary>
    public int CharacterActionId { get; set; }

    /// <summary>
    /// Name of character that login or logout
    /// </summary>
    public string CharacterName { get; set; }

    /// <summary>
    /// Id of specific World Scan
    /// </summary>
    public int WorldScanId { get; set; }

    /// <summary>
    /// Boolean (true if character just login, otherwise false)
    /// </summary>
    public bool IsOnline { get; set; }

    /// <summary>
    /// Id of specific world
    /// </summary>
    public short WorldId { get; set; }

    /// <summary>
    /// Date when character is logout or login
    /// </summary>
    public DateOnly LogoutOrLoginDate { get; set; }

    // Associations
    public WorldScan WorldScan { get; set; }
    public World World { get; set; }

}
