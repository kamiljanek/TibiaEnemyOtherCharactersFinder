using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;

public class TibiaDataV3WorldResponse
{
    [JsonProperty("information")]
    public InformationResponseV3 Information { get; set; }

    [JsonProperty("worlds")]
    public WorldInfoResponseV3 Worlds { get; set; }
}

public class WorldInfoResponseV3
{
    [JsonProperty("world")]
    public WorldResponseV3 World { get; set; }
}








