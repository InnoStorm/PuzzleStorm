using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client {
    public class LobbyPageViewModel : BaseViewModel {
        #region Properties

        

        #endregion

        #region Commands

        public ICommand StartReadyCommand { get; set; }


        #endregion

        #region Constuctors

        public LobbyPageViewModel() {
            StartReadyCommand = new RelayCommand(() => StartReadyGame());
        }

        #endregion

        #region Metods


        //Create button f-ja
        public void StartReadyGame() {

        }
        #endregion
    }
}
