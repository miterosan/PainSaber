
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
            var duration = PainSaberConfig.Instance.NoteMissed.DurationMs;
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);

            foreach (var name in PainSaberConfig.Instance.NoteMissed.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)PainSaberConfig.Instance.NoteMissed.Intensity
                ));
            }
        }

        public static void NoteIncorrect()
        {
            var duration = PainSaberConfig.Instance.NoteFailed.DurationMs;
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);

            foreach (var name in PainSaberConfig.Instance.NoteFailed.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)PainSaberConfig.Instance.NoteFailed.Intensity
                ));
            }
        }

        public static void HeadInWall(long durationMs)
        {
            int intensity = PainSaberConfig.Instance.HeadInWall.StartIntensity;
            int incrementBy = PainSaberConfig.Instance.HeadInWall.IncrementBy;
            int incrementEvery = PainSaberConfig.Instance.HeadInWall.IncrementEveryMs;

            intensity += (int)(durationMs / incrementEvery) * incrementBy;

            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(100 + incrementEvery);

            foreach (var name in PainSaberConfig.Instance.HeadInWall.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)intensity
                ));
            }
        }

        public static void BombCut() 
        {
            var duration = PainSaberConfig.Instance.BombCut.DurationMs;
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);

            foreach (var name in PainSaberConfig.Instance.BombCut.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)PainSaberConfig.Instance.BombCut.Intensity
                ));
            }
        }

        [Init]
        public PainSaberPlugin(Logger logger, Config conf)
        {
            Log = logger;
            PainSaberConfig.Instance = conf.Generated<PainSaberConfig>();

            API = new OpenShockApi(PainSaberConfig.Instance.OpenShockApiKey);
            RealTimeApi = new RealTimeApi(PainSaberConfig.Instance.OpenShockApiKey);

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

        [OnExit]
        public void OnExit()
        {
            // teardown
        }


        private static string getShockerIdByName(string name) 
        {
            // todo make faster
            return Shocker.Find(s => s.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)).id;
        }

    }
}

