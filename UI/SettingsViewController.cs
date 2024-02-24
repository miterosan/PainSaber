using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using PainSaber.OpenShock.RestModels;
using PainSaber.Utils;

namespace PainSaber.UI
{
    public class SettingsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "PainSaber.UI.Settings.bsml";



        private string statusText;
        [UIValue("status-text")]
        public string StatusText
        {
            get => statusText;
            set
            {
                statusText = value;
                NotifyPropertyChanged();
            }
        }

        // Note missed config
        [UIValue("note-missed-intensity")]
        public int NoteMissedIntensity
        {
            get => PainSaberPlugin.Config.NoteMissed.Intensity;
            set
            {
                PainSaberPlugin.Config.NoteMissed.Intensity = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("note-missed-duration")]
        public int NoteMissedDuration
        {
            get => PainSaberPlugin.Config.NoteMissed.DurationMs;
            set
            {
                PainSaberPlugin.Config.NoteMissed.DurationMs = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("note-missed-shockers")]
        private NotifyingProperty<bool>[] noteMissedShockers;
        public NotifyingProperty<bool>[] NoteMissedShocker
        {
            get => noteMissedShockers;
            set
            {
                noteMissedShockers = value;
                NotifyPropertyChanged();
            }
        }

        // Note failed config

        [UIValue("note-failed-intensity")]
        public int NoteFailedIntensity
        {
            get => PainSaberPlugin.Config.NoteFailed.Intensity;
            set
            {
                PainSaberPlugin.Config.NoteFailed.Intensity = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("note-failed-duration")]
        public int NoteFailedDuration
        {
            get => PainSaberPlugin.Config.NoteFailed.DurationMs;
            set
            {
                PainSaberPlugin.Config.NoteFailed.DurationMs = value;
                NotifyPropertyChanged();
            }
        }

        private NotifyingProperty<bool>[] noteFailedShockers;
        [UIValue("note-failed-shockers")]
        public NotifyingProperty<bool>[] NoteFailedShockers
        {
            get => noteFailedShockers;
            set
            {
                noteFailedShockers = value;
                NotifyPropertyChanged();
            }
        }


        // Bomb hit config

        [UIValue("bomb-hit-intensity")]
        public int BombHitIntensity
        {
            get => PainSaberPlugin.Config.BombCut.Intensity;
            set
            {
                PainSaberPlugin.Config.BombCut.Intensity = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("bomb-hit-duration")]
        public int BombHitDuration
        {
            get => PainSaberPlugin.Config.BombCut.DurationMs;
            set
            {
                PainSaberPlugin.Config.BombCut.DurationMs = value;
                NotifyPropertyChanged();
            }
        }

        private NotifyingProperty<bool>[] bombHitShockers;
        [UIValue("bomb-hit-shockers")]
        public NotifyingProperty<bool>[] BombHitShockers
        {
            get => bombHitShockers;
            set
            {
                bombHitShockers = value;
                NotifyPropertyChanged();
            }
        }

        // Head in wall settings

        [UIValue("head-in-wall-start-intensity")]
        public int WallStartIntensity
        {
            get => PainSaberPlugin.Config.HeadInWall.StartIntensity;
            set
            {
                PainSaberPlugin.Config.HeadInWall.StartIntensity = value;
                NotifyPropertyChanged();
            }
        }


        [UIValue("head-in-wall-increase-by")]
        public int WallIncreaseBy
        {
            get => PainSaberPlugin.Config.HeadInWall.IncrementBy;
            set
            {
                PainSaberPlugin.Config.HeadInWall.IncrementBy = value;
                NotifyPropertyChanged();
            }
        }

        [UIValue("head-in-wall-increase-every")]
        public int WallIncreaseEveryMs
        {
            get => PainSaberPlugin.Config.HeadInWall.IncrementEveryMs;
            set
            {
                PainSaberPlugin.Config.HeadInWall.IncrementEveryMs = value;
                NotifyPropertyChanged();
            }
        }

        private NotifyingProperty<bool>[] wallShockers;
        [UIValue("head-in-wall-shockers")]
        public NotifyingProperty<bool>[] WallShockers
        {
            get => wallShockers;
            set
            {
                wallShockers = value;
                NotifyPropertyChanged();
            }
        }

        void Awake()
        {
            PainSaberPlugin.Log.Debug("SettingsViewController awoken");

            PainSaberPlugin.Status.PropertyChanged += (a,b) =>
            {
                StatusText = PainSaberPlugin.Status.Value.ToString();
            };

            UpdateShockers();
        }

        void UpdateShockers()
        {
            NotifyingProperty<bool>[] asNotifyingBool(Shocker[] shockers, List<string> configShockers)
            {
                return shockers.Select(s => new NotifyingProperty<bool>{
                    Name = s.name,
                    Value = configShockers.Exists(sc => sc.Equals(s.name, StringComparison.InvariantCultureIgnoreCase)),
                    OnChange = SaveShockers
                }).ToArray();
            }

            noteMissedShockers = asNotifyingBool(PainSaberPlugin.Shockers, PainSaberPlugin.Config.NoteMissed.Shockers);
            noteFailedShockers = asNotifyingBool(PainSaberPlugin.Shockers, PainSaberPlugin.Config.NoteFailed.Shockers);
            BombHitShockers = asNotifyingBool(PainSaberPlugin.Shockers, PainSaberPlugin.Config.BombCut.Shockers);
            WallShockers = asNotifyingBool(PainSaberPlugin.Shockers, PainSaberPlugin.Config.HeadInWall.Shockers);
        }

        void SaveShockers()
        {
            PainSaberPlugin.Config.NoteMissed.Shockers = NoteMissedShocker.Where(s => s.Value).Select(s => s.Name).ToList();
            PainSaberPlugin.Config.NoteFailed.Shockers = NoteFailedShockers.Where(s => s.Value).Select(s => s.Name).ToList();
            PainSaberPlugin.Config.BombCut.Shockers = BombHitShockers.Where(s => s.Value).Select(s => s.Name).ToList();
            PainSaberPlugin.Config.HeadInWall.Shockers = WallShockers.Where(s => s.Value).Select(s => s.Name).ToList();
        }
    }
}