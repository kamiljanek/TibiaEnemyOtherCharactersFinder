using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;

public class TibiaDataV3WorldResponse
{
    [JsonProperty("information")]
    public InformationResponseV3 Information { get; set; }

    [JsonProperty("worlds")]
    public WorldResponseV3 Worlds { get; set; }
}








