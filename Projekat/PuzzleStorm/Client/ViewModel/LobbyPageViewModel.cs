using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client {
    public class LobbyPageViewModel : BaseViewModel {
        #region Properties

        public List<LobbyJoinedPlayerViewModel> JoinedPlayersItems { get; set; }

        #endregion

        #region Commands

        public ICommand StartReadyCommand { get; set; }


        #endregion

        #region Constuctors

        public LobbyPageViewModel() {
            StartReadyCommand = new RelayCommand(StartReadyGame);

            JoinedPlayersItems = new List<LobbyJoinedPlayerViewModel>()
            {
                new LobbyJoinedPlayerViewModel()
                {
                    Username = "Pera1",
                    Ready = true
                },
                new LobbyJoinedPlayerViewModel()
                {
                    Username = "Pera2",
                    Ready = false
                },
                new LobbyJoinedPlayerViewModel()
                {
                    Username = "Pera3",
                    Ready = false
                }
            };
        }

        #endregion

        #region Metods


        //Create button f-ja
        public void StartReadyGame() {

        }
        #endregion
    }
}
