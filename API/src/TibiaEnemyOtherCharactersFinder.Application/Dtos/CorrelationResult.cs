namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class CorrelationResult
{
    public string OtherCharacterName { get; init; }
    public short NumberOfMatches { get; init; }
    public DateOnly FirstMatchDateOnly => DateOnly.FromDateTime(CreateDate);
    public DateOnly LastMatchDateOnly => DateOnly.FromDateTime(LastMatchDate);

    private DateTime CreateDate { get; init; }
    private DateTime LastMatchDate { get; init; }
}