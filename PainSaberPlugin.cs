
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using PainSaber.Hooks;
using PainSaber.OpenShock;
using PainSaber.OpenShock.Realtime;
using PainSaber.OpenShock.RestModels;

namespace PainSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class PainSaberPlugin
    {
        public static Logger Log { get; private set; }
        public static OpenShockApi API { get; private set; }
        public static RealTimeApi RealTimeApi { get; private set; }

        private static readonly List<Shocker> Shocker = new List<Shocker>();

        public static void NoteMissed() 
        {
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(500);

            foreach (var shocker in Shocker)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(shocker.id, endTime, 10));
            }
        }

        public static void NoteIncorrect()
        {
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(500);

            foreach (var shocker in Shocker)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(shocker.id, endTime, 10));
            }
        }

        public static void HeadInWall(long durationMs)
        {
            //todo implement
        }

        public static void BombCut() 
        {
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(500);

            foreach (var shocker in Shocker)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(shocker.id, endTime, 10));
            }
        }

        [Init]
        public PainSaberPlugin(Logger logger, Config conf)
        {
            Log = logger;
            PainSaberConfig.Instance = conf.Generated<PainSaberConfig>();

            string key = "";

            API = new OpenShockApi(key);
            RealTimeApi = new RealTimeApi(key);

            EventRegistrationBehaviour.OnLoad();

            

            AsyncUtils.RunWithCallback(API.GetOwnShockers(), devices => {
                var device = devices.First();

                Shocker.AddRange(device.shockers); // todo multiple devices

                Log.Debug("Received Infos about shockers");

                AsyncUtils.RunWithCallback(API.GetLiveControlGatewayInfo(device.id), info => {
                    AsyncUtils.FireAndForget(RealTimeApi.Connect(device.id, info.Gateway));
                    Log.Debug("Got Gateway info");
                });
            });          
        }

        [OnStart]
        public void OnStart()
        {
            new Harmony("org.miterosan.painsaber").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}

