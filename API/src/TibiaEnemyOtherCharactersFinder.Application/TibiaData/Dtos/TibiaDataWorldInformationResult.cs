namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaDataWorldInformationResult
{
    public WorldApiResult world { get; set; }
    public InformationApiResultWorld information { get; set; }
}

public class WorldApiResult
{
    public string battleye_date { get; set; }
    public bool battleye_protected { get; set; }
    public string creation_date { get; set; }
    public string game_world_type { get; set; }
    public string location { get; set; }
    public string name { get; set; }
    public List<OnlinePlayer> online_players { get; set; }
    public int players_online { get; set; }
    public bool premium_only { get; set; }
    public string pvp_type { get; set; }
    public string record_date { get; set; }
    public int record_players { get; set; }
    public string status { get; set; }
    public string tournament_world_type { get; set; }
    public string transfer_type { get; set; }
    public List<string> world_quest_titles { get; set; }
}

public class InformationApiResultWorld
{
    public ApiResultWorld api { get; set; }
    public StatusApiResultWorld status { get; set; }
    public string timestamp { get; set; }
}

public class ApiResultWorld
{
    public string commit { get; set; }
    public string release { get; set; }
    public int version { get; set; }
}

public class OnlinePlayer
{
    public int level { get; set; }
    public string name { get; set; }
    public string vocation { get; set; }
}

public class StatusApiResultWorld
{
    public int error { get; set; }
    public int http_code { get; set; }
    public string message { get; set; }
}








