using System.Collections.Generic;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;

namespace PainSaber
{
    public class PainSaberConfig
    {
        public static PainSaberConfig Instance { get; set; }

        public virtual string OpenShockApiKey { get; set; } = "Your APIKEY here";
        public virtual string Device { get; set; } = "The id of the control box";

        [NonNullable]
        public virtual ShocksConfig NoteMissed { get; set; } = new ShocksConfig();
        [NonNullable]
        public virtual ShocksConfig NoteFailed { get; set; } = new ShocksConfig();
        [NonNullable]
        public virtual ShocksConfig BombCut { get; set; } = new ShocksConfig();
        [NonNullable]
        public virtual WallShocksConfig HeadInWall { get; set; } = new WallShocksConfig();


        public class ShocksConfig 
        {
            [UseConverter(typeof(ListConverter<string>))]
            public virtual List<string> Shockers { get; set; } = new List<string>();

            public virtual int Intensity { get; set; } = 0;

            public virtual int DurationMs { get; set; } = 0;
        }

        public class WallShocksConfig
        {
            [UseConverter(typeof(ListConverter<string>))]
            public virtual List<string> Shockers { get; set; } = new List<string>();

            public virtual int StartIntensity { get; set; } = 0;

            public virtual int IncrementBy { get; set; } = 0;

            public virtual int IncrementEveryMs { get; set; } = 0;
        }
    }
}