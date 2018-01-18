using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Client {

    public class RoomsListViewModel : BaseViewModel {

        #region Properties

        public List<RoomsPropsViewModel> RoomsItemsList { get; set; }

        #endregion

        #region Commands

        public ICommand BackCommand { get; set; }
        
        #endregion

        #region Constuctors

        public RoomsListViewModel() {
            RoomsItemsList = new List<RoomsPropsViewModel>()
            {
                new RoomsPropsViewModel()
                {
                    Name = "Test1",
                    By = "Ja",
                    Difficulty = "16",
                    MaxPlayers = "5",
                    Rounds = "2"
                },

                new RoomsPropsViewModel()
                {
                    Name = "Test2",
                    By = "Ti",
                    Difficulty = "25",
                    MaxPlayers = "4",
                    Rounds = "2"
                },
                new RoomsPropsViewModel()
                {
                    Name = "Test3",
                    By = "On",
                    Difficulty = "36",
                    MaxPlayers = "2",
                    Rounds = "1"
                },
                new RoomsPropsViewModel()
                {
                    Name = "Test4",
                    By = "Ono",
                    Difficulty = "25",
                    MaxPlayers = "5",
                    Rounds = "3"
                }
            };

            BackCommand = new RelayCommand(BackToMainPage);

        }

        #endregion

        #region Metods

        public void BackToMainPage()
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        #endregion
    }
}
