using Newtonsoft.Json;

namespace TibiaEnemyOtherCharactersFinder.Application.TibiaData.Dtos.v4;

public class InformationResponseV4
{
    [JsonProperty("api")]
    public ApiResponseV4 Api { get; set; }

    [JsonProperty("status")]
    public StatusResponseV4 Status { get; set; }

    [JsonProperty("timestamp")]
    public string Timestamp { get; set; }
}

public class ApiResponseV4
{
    [JsonProperty("commit")]
    public string Commit { get; set; }

    [JsonProperty("release")]
    public string Release { get; set; }

    [JsonProperty("version")]
    public long Version { get; set; }
}

public class StatusResponseV4
{
    [JsonProperty("error")]
    public long Error { get; set; }

    [JsonProperty("http_code")]
    public long HttpCode { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }
}





