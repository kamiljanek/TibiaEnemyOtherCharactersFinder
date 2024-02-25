using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;

public class TibiaDataV3WorldsResponse
{
    [JsonProperty("information")]
    public InformationResponseV3 Information { get; set; }

    [JsonProperty("worlds")]
    public WorldsResponseV3 Worlds { get; set; }
}

public class WorldsResponseV3
{
    [JsonProperty("players_online")]
    public int PlayersOnline { get; set; }

    [JsonProperty("record_date")]
    public string RecordDate { get; set; }

    [JsonProperty("record_players")]
    public int RecordPlayers { get; set; }

    [JsonProperty("regular_worlds")]
    public WorldResponseV3[] RegularWorlds { get; set; }

    [JsonProperty("tournament_worlds")]
    public WorldResponseV3[] TournamentWorlds { get; set; }
}





