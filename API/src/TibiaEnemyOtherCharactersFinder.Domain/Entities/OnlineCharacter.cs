namespace TibiaEnemyOtherCharactersFinder.Domain.Entities;

public class OnlineCharacter
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
    /// Datetime when character was online
    /// </summary>
    public DateTime OnlineDateTime { get; }

    public OnlineCharacter(string name, string worldName)
    {
        Name = name;
        WorldName = worldName;
        OnlineDateTime = DateTime.UtcNow;
    }
}
