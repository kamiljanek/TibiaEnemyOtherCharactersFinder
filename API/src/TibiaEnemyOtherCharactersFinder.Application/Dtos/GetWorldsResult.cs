namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class GetWorldsResult
{
    public int Count => Worlds.Count;
    public IReadOnlyList<WorldResult> Worlds { get; set; }
}