namespace TibiaEnemyOtherCharactersFinder.Domain.Entities;

public class CharacterCorrelation : IEntity
{
    /// <summary>
    /// Id of specific correlation between two characters
    /// </summary>
    public int CorrelationId { get; set; }

    /// <summary>
    /// Id of specific character that logout
    /// </summary>
    public int LogoutCharacterId { get; set; }

    /// <summary>
    /// Id of specific character that login
    /// </summary>
    public int LoginCharacterId { get; set; }

    /// <summary>
    /// Quantity occurrence of combination
    /// </summary>
    public short NumberOfMatches { get; set; }

    /// <summary>
    /// Date of first occurance
    /// </summary>
    public DateOnly CreateDate { get; set; }

    /// <summary>
    /// Date of last occurance
    /// </summary>
    public DateOnly LastMatchDate { get; set; }

    // Associations
    public Character LogoutCharacter { get; set; }
    public Character LoginCharacter { get; set; }
}
