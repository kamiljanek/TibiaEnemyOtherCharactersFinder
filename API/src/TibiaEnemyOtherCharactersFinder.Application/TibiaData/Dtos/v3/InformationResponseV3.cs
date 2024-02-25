using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v3;

public class InformationResponseV3
{
    [JsonProperty("api_version")]
    public int ApiVersion { get; set; }

    [JsonProperty("timestamp")]
    public string Timestamp { get; set; }
}





