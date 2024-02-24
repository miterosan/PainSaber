using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PainSaber.Utils
{
    public class NotifyingProperty<T> : INotifyPropertyChanged
    {
        public string Name { get; set; }

        private T value;
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                try
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    OnChange?.Invoke();
                } catch(Exception ex)
                {
                    PainSaberPlugin.Log.Error($"Error Invoking PropertyChanged: {ex.Message}");
                    PainSaberPlugin.Log.Error(ex);
                }
            }
        }

        public Action OnChange { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}