using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v4;

public class WorldResponseV4
{
    [JsonProperty("battleye_date")]
    public string BattleyeDate { get; set; }

    [JsonProperty("battleye_protected")]
    public bool BattleyeProtected { get; set; }

    [JsonProperty("creation_date")]
    public string CreationDate { get; set; }

    [JsonProperty("game_world_type")]
    public string GameWorldType { get; set; }

    [JsonProperty("location")]
    public string Location { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("online_players")]
    public OnlinePlayerResponseV4[] OnlinePlayers { get; set; }

    [JsonProperty("players_online")]
    public int PlayersOnline { get; set; }

    [JsonProperty("premium_only")]
    public bool PremiumOnly { get; set; }

    [JsonProperty("pvp_type")]
    public string PvpType { get; set; }

    [JsonProperty("record_date")]
    public string RecordDate { get; set; }

    [JsonProperty("record_players")]
    public int RecordPlayers { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("tournament_world_type")]
    public string TournamentWorldType { get; set; }

    [JsonProperty("transfer_type")]
    public string TransferType { get; set; }

    [JsonProperty("world_quest_titles")]
    public List<string> WorldQuestTitles { get; set; }
}

public class OnlinePlayerResponseV4
{
    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("vocation")]
    public string Vocation { get; set; }
}







