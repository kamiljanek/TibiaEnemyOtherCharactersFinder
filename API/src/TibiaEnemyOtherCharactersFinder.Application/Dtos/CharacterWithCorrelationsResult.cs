using System.Text.Json.Serialization;

namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class CharacterWithCorrelationsResult
{
    [JsonPropertyName("Other character name")]
    public string OtherCharacterName { get; set; }

    [JsonPropertyName("Number of matches")]
    public short NumberOfMatches { get; set; }

    [JsonPropertyName("First match date")]
    public DateOnly CreateDateOnly => DateOnly.FromDateTime(CreateDate);

    [JsonPropertyName("Last match date")]
    public DateOnly LastMatchDateOnly => DateOnly.FromDateTime(LastMatchDate);

    private DateTime CreateDate { get; set; }
    private DateTime LastMatchDate { get; set; }
}