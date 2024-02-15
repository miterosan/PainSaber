using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PainSaber
{
    public class PainSaberConfig
    {
        public static PainSaberConfig Instance { get; set; }

        public virtual string OpenShockApiKey { get; set; } = "Your APIKEY here";
    }
}