using System.ComponentModel;
using PropertyChanged;

namespace Client {

    /// <summary>
    /// Base viewmodel koji implementira property changed event
    /// </summary>

    [AddINotifyPropertyChangedInterface]
    public class BaseViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Event koji se pozove kad se neki property promeni
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        /// <summary>
        /// Zove se ovo, a ona pozove PropertyChanged
        /// </summary>
        /// <param name="name"></param>
        public void OnPropertyChanged(string name) {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
