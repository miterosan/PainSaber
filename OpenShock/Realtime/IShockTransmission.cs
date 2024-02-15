using System;

namespace PainSaber.OpenShock.Realtime
{
    public interface IShockTransmission
    {
        /// <summary>
        /// The time when this transmission ends at the latest.
        /// Is UTC.
        /// </summary>
        DateTime Endtime { get; }

        /// <summary>
        /// The id of the shocker
        /// </summary>
        string ShockerId { get; }

        /// <summary>
        /// The intensity at the 
        /// </summary>
        /// <returns>The intensity ranging from 0 to 99</returns>
        byte GetIntensity();
    }
}