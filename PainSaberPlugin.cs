
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Settings;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using ModestTree;
using PainSaber.Hooks;
using PainSaber.OpenShock;
using PainSaber.OpenShock.Realtime;
using PainSaber.OpenShock.RestModels;
using PainSaber.Utils;

namespace PainSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class PainSaberPlugin
    {
        public static Logger Log { get; private set; }
        public static OpenShockApi API { get; private set; }
        public static RealTimeApi RealTimeApi { get; private set; }
        public static PainSaberConfig Config { get; private set; }
        public static Device[] Devices { get; private set; } = Array.Empty<Device>();
        public static Shocker[] Shockers => SelectedDevice?.shockers ?? Array.Empty<Shocker>();
        public static Device SelectedDevice { get; private set; }
        public static NotifyingProperty<PainSaberPluginStatus> Status { get; } = new NotifyingProperty<PainSaberPluginStatus>();

        private static Timer retryTimer = new Timer(5000);

        private static void retryConnection()
        {
            // is connected? stop right here
            if (RealTimeApi.State == RealTimeApiState.Connected) return;

            AsyncUtils.FireAndForget(new Task(async () => await tryReconnect()));
        }

        private static async Task tryReconnect()
        {
            // validate apikey
            API = new OpenShockApi(Config.OpenShockApiKey);
            RealTimeApi = new RealTimeApi(Config.OpenShockApiKey);

            try
            {
                Devices = await API.GetOwnShockers();
            }
            catch
            {
                Status.Value = PainSaberPluginStatus.InvalidApiKey;
                return;
            }

            if (Devices.IsEmpty())
            {
                Status.Value = PainSaberPluginStatus.NoDevicesAvailable;
                return;
            }

            // todo support selecting the device from ingame UI
            SelectedDevice = Devices.FirstOrDefault();

            string gateway;

            Status.Value = PainSaberPluginStatus.ConnectingToDevice;

            try
            {
                var response = await API.GetLiveControlGatewayInfo(SelectedDevice.id);
                gateway = response.Gateway;
            }
            catch
            {
                Status.Value = PainSaberPluginStatus.NoDevicesAvailable;
                return;
            }

            try
            {
                // connect to realtime api
                await RealTimeApi.Connect(SelectedDevice.id, gateway);
            }
            catch
            {
                Status.Value = PainSaberPluginStatus.DeviceConnectionFailed;
                return;
            }

            // todo devicestate etc

        }

        public static void NoteMissed()
        {
            var duration = Config.NoteMissed.DurationMs;
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);

            foreach (var name in Config.NoteMissed.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)Config.NoteMissed.Intensity
                ));
            }
        }

        public static void NoteIncorrect()
        {
            var duration = Config.NoteFailed.DurationMs;
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);

            foreach (var name in Config.NoteFailed.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)Config.NoteFailed.Intensity
                ));
            }
        }

        public static void HeadInWall(long durationMs)
        {
            int intensity = Config.HeadInWall.StartIntensity;
            int incrementBy = Config.HeadInWall.IncrementBy;
            int incrementEvery = Config.HeadInWall.IncrementEveryMs;

            intensity += (int)(durationMs / incrementEvery) * incrementBy;

            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(100 + incrementEvery);

            foreach (var name in Config.HeadInWall.Shockers)
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
            var duration = Config.BombCut.DurationMs;
            var endTime = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);

            foreach (var name in Config.BombCut.Shockers)
            {
                RealTimeApi.AddShockTransmission(new ConstantShockTransmission(
                    getShockerIdByName(name),
                    endTime,
                    (byte)Config.BombCut.Intensity
                ));
            }
        }



        [Init]
        public PainSaberPlugin(Logger logger, IPA.Config.Config conf)
        {
            Log = logger;
            Config = conf.Generated<PainSaberConfig>();

            EventRegistrationBehaviour.OnLoad();

            retryTimer.Elapsed += (a,b) => retryConnection();
            retryTimer.Start();

            BSMLSettings.instance.AddSettingsMenu("Painsaber", "PainSaber.UI.Settings.bsml", new UI.SettingsViewController());
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
            return Shockers.FirstOrDefault(s => s.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))?.id;
        }

    }
}

