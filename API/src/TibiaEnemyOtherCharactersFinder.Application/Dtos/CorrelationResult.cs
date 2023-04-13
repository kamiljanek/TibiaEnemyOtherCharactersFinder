using System.Text.Json.Serialization;

namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class CorrelationResult
{
    public string OtherCharacterName { get; set; }
    public short NumberOfMatches { get; set; }

    [JsonPropertyName("First match date")]
    public DateOnly CreateDateOnly => DateOnly.FromDateTime(CreateDate);

    [JsonPropertyName("Last match date")]
    public DateOnly LastMatchDateOnly => DateOnly.FromDateTime(LastMatchDate);

    private DateTime CreateDate { get; set; }
    private DateTime LastMatchDate { get; set; }
}