namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class GetActiveWorldsResult
{
    public int Count => Worlds.Count;
    public IReadOnlyList<ActiveWorldResult> Worlds { get; set; }
}