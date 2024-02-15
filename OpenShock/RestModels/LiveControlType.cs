using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PainSaber.OpenShock.RestModels
{

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LiveControlType
    {
        Stop = 0,
        Shock = 1,
        Vibrate = 2,
        Sound = 3
    }
}