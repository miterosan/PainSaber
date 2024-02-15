using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PainSaber.OpenShock.RestModels
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LiveRequestType
    {
        Frame = 0,
    
        Pong = 1000
    }
}