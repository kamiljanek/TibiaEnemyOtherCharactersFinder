namespace TibiaEnemyOtherCharactersFinder.Application.Dtos;

public class GetActiveWorldsResult
{
    public int Count => ActiveWorlds.Count;
    public IReadOnlyList<ActiveWorldResult> ActiveWorlds { get; set; }
}