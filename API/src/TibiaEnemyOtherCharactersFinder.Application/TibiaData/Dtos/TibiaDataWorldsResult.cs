namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaDataWorldsResult
{
    public ApiWorldsResult worlds { get; set; }
    public WorldsInformationResult information { get; set; }
}
public class ApiWorldsResult
{
    public int players_online { get; set; }
    public int record_players { get; set; }
    public DateTime record_date { get; set; }
    public List<RegularWorldResult> regular_worlds { get; set; }
    public object tournament_worlds { get; set; }
}
public class WorldsInformationResult
{
    public int api_version { get; set; }
    public DateTime timestamp { get; set; }
}

public class RegularWorldResult
{
    public string name { get; set; }
    public string status { get; set; }
    public int players_online { get; set; }
    public string location { get; set; }
    public string pvp_type { get; set; }
    public bool premium_only { get; set; }
    public string transfer_type { get; set; }
    public bool battleye_protected { get; set; }
    public string battleye_date { get; set; }
    public string game_world_type { get; set; }
    public string tournament_world_type { get; set; }
}


