using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace PainSaber.OpenShock.RestModels
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LiveResponseType
    {
        Frame = 0,
    
        DeviceNotConnected = 100,
        DeviceConnected = 101,
        ShockerNotFound = 150,
        ShockerMissingLivePermission = 151,
        ShockerMissingPermission = 152,
        
        InvalidData = 200,
        RequestTypeNotFound = 201,
        
        Ping = 1000,
        LatencyAnnounce = 1001
    }
}