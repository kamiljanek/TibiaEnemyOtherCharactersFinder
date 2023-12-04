namespace TibiaEnemyOtherCharactersFinder.Domain.Entities;

public class TrackedCharacter
{
    /// <summary>
    /// Character name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// World name
    /// </summary>
    public string WorldName { get; }

    /// <summary>
    /// Identifier of the connection from which it starts tracing this character
    /// </summary>
    public string ConnectionId { get; }

    /// <summary>
    /// Datetime when character started being tracked
    /// </summary>
    public DateTime StartTrackDateTime { get; }

    public TrackedCharacter(string name, string worldName, string connectionId)
    {
        Name = name;
        WorldName = worldName;
        ConnectionId = connectionId;
        StartTrackDateTime = DateTime.UtcNow;
    }
}
