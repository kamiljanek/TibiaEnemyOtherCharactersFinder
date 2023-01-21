namespace TibiaEnemyOtherCharactersFinder.Infrastructure.Entities;

public class WorldScan : IEntity
{
    /// <summary>
    /// Id of World Scan
    /// </summary>
    public int WorldScanId { get; set; }

    /// <summary>
    /// List of online characters
    /// </summary>
    public string CharactersOnline { get; set; }

    /// <summary>
    /// Id of specific world
    /// </summary>
    public short WorldId { get; set; }

    /// <summary>
    /// Boolean (true if World Scan is deleted, otherwise false)
    /// </summary>
    public bool IsDeleted { get; set; }

    /// <summary>
    /// Date and time when World Scan was created
    /// </summary>
    public DateTime ScanCreateDateTime { get; set; }

    // Associations
    public World World { get; set; }
}
