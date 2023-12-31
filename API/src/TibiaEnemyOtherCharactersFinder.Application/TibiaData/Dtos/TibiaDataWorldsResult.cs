namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos;

public class TibiaDataWorldsResult
{
    public Worlds worlds { get; set; }
    public InformationApiResultWorlds information { get; set; }
}

public class Worlds
{
    public int players_online { get; set; }
    public string record_date { get; set; }
    public int record_players { get; set; }
    public List<RegularWorld> regular_worlds { get; set; }
    public List<TournamentWorld> tournament_worlds { get; set; }
}

public class TournamentWorld
{
    public string battleye_date { get; set; }
    public bool battleye_protected { get; set; }
    public string game_world_type { get; set; }
    public string location { get; set; }
    public string name { get; set; }
    public int players_online { get; set; }
    public bool premium_only { get; set; }
    public string pvp_type { get; set; }
    public string status { get; set; }
    public string tournament_world_type { get; set; }
    public string transfer_type { get; set; }
}

public class RegularWorld
{
    public string battleye_date { get; set; }
    public bool battleye_protected { get; set; }
    public string game_world_type { get; set; }
    public string location { get; set; }
    public string name { get; set; }
    public int players_online { get; set; }
    public bool premium_only { get; set; }
    public string pvp_type { get; set; }
    public string status { get; set; }
    public string tournament_world_type { get; set; }
    public string transfer_type { get; set; }
}

public class InformationApiResultWorlds
{
    public ApiApiResultWorlds api { get; set; }
    public StatusApiResultWorlds status { get; set; }
    public string timestamp { get; set; }
}

public class ApiApiResultWorlds
{
    public string commit { get; set; }
    public string release { get; set; }
    public int version { get; set; }
}

public class StatusApiResultWorlds
{
    public int error { get; set; }
    public int http_code { get; set; }
    public string message { get; set; }
}





