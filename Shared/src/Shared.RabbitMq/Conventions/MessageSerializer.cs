using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Shared.RabbitMQ.Conventions;

public sealed class MessageSerializer
{
    private readonly JsonSerializerSettings _settings;

    public MessageSerializer()
    {
        _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
    }

    public string Serialize(object value) => JsonConvert.SerializeObject(value, _settings);

    public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value, _settings) ?? throw new InvalidOperationException();
}