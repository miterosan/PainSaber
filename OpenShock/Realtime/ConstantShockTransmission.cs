using System;

namespace PainSaber.OpenShock.Realtime
{
    public class ConstantShockTransmission : IShockTransmission
    {

        public ConstantShockTransmission(string shockerId, DateTime endTime, byte intensity)
        {
            ShockerId = shockerId;
            Endtime = endTime;
            Intensity = intensity;
        }

        public DateTime Endtime { get; set; }

        public string ShockerId { get; private set; }

        public byte Intensity { get; set; }

        public byte GetIntensity()
        {
            return Intensity;
        }
    }
}