using PainSaber.OpenShock.RestModels;

namespace PainSaber.OpenShock
{
    public class ControlRequest
    {
        public Shocker Shocker { get; set; }
        public ShockerCommandType Type { get; set; }
        public int Amount { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; }
    }
}