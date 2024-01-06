using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BattleShipV2 {
    public class ViewModelBase : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void Set<T>(ref T field, T value, [CallerMemberName] string propName = "") {
            if (!field.Equals(value)) {
                field = value;
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        protected void Set<T>(ref T field, T value, params string[] propNames) {
            if (!field.Equals(value)) {
                field = value;
                foreach (var name in propNames)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        protected void Notify(params string[] names) {
            foreach (var name in names)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
