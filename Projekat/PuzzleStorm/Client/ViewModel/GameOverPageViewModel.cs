using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Client {

    public class GameOverPageViewModel : BaseViewModel {

        #region Properties

        public List<GameOverItemViewModel> GameOverScoreItems { get; set; }

        #endregion

        #region Commands

        public ICommand BackToMainPage { get; set; }

        #endregion

        #region Constructors

        public GameOverPageViewModel()
        {
            GameOverScoreItems = new List<GameOverItemViewModel>()
            {
                new GameOverItemViewModel()
                {
                    No = "#1",
                    UserName = "Pera1",
                    Score = "185"
                },
                new GameOverItemViewModel()
                {
                    No = "#2",
                    UserName = "Pera2",
                    Score = "170"
                },
                new GameOverItemViewModel()
                {
                    No = "#3",
                    UserName = "Pera3",
                    Score = "155"
                }
            };

            BackToMainPage = new RelayCommand(BackToMain);

        }

        #endregion

        #region Metods

        public void BackToMain()
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        #endregion
    }
}
