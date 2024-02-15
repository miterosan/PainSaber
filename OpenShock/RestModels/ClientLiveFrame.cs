using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PainSaber.OpenShock.RestModels
{
    public class ClientLiveFrame
    {
        public Guid Shocker { get; set; }
        public byte Intensity { get; set; }
        public LiveControlType Type { get; set; }

    }
}