using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v4;

public class TibiaDataV4WorldsResponse
{
    [JsonProperty("worlds")]
    public WorldsResponseV4 Worlds { get; set; }

    [JsonProperty("information")]
    public InformationResponseV4 Information { get; set; }
}

public class WorldsResponseV4
{
    [JsonProperty("players_online")]
    public int PlayersOnline { get; set; }

    [JsonProperty("record_date")]
    public string RecordDate { get; set; }

    [JsonProperty("record_players")]
    public int RecordPlayers { get; set; }

    [JsonProperty("regular_worlds")]
    public WorldResponseV4[] RegularWorlds { get; set; }

    [JsonProperty("tournament_worlds")]
    public WorldResponseV4[] TournamentWorlds { get; set; }
}





