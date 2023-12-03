namespace TibiaEnemyOtherCharactersFinder.Application.Services;

public interface ITrackCharacterService
{
    Task CreateTrack(string characterName, string connectionId);
    Task RemoveTrack(string characterName, string connectionId);
    Task RemoveTracksByConnectionId(string connectionId);
}