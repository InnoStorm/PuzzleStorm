using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace Client {

    public class GameOverPageViewModel : BaseViewModel {

        #region Properties

        public ObservableCollection<GameOverItemViewModel> GameOverScoreItems { get; set; }

        public string ButtonText { get; set; }

        public string Title { get; set; }

        #endregion

        #region Commands

        public ICommand BackOrStartCommand { get; set; }

        #endregion

        #region Constructors

        public GameOverPageViewModel()
        {

            /*
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
            */

            GameOverScoreItems = new ObservableCollection<GameOverItemViewModel>();

            Player.Instance.InGame.Score.Scores.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            int i = 1;

            foreach (var s in Player.Instance.InGame.Score.Scores)
            {
                GameOverScoreItems.Add(new GameOverItemViewModel()
                {
                    No = "#" + i,
                    Score = s.Item2.ToString(),
                    UserName = s.Item1.Username
                });

                i++;
            }

            if (Player.Instance.InGame.Over)
            {
                Title = "GAME OVER";
                ButtonText = "BACK TO MAIN MANU";
                BackOrStartCommand = new RelayCommand(BackToMain);
            }
            else
            {
                Title = "ROUND OVER";

                if (Player.Instance.Creator)
                {
                    BackOrStartCommand = new RelayCommand(StartNewRound);
                    ButtonText = "START NEW ROUND";
                }
                else
                {
                    BackOrStartCommand = new RelayCommand(WaitForRoundAsync);
                    ButtonText = "WAIT FOR NEW ROUND";
                }
            }
        }

        #endregion

        #region Metods

        public void BackToMain()
        {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        public void StartNewRound() {
            
        }

        public async void WaitForRoundAsync() {
            await DialogHost.Show(new SampleMessageDialog {
                Message = { Text = "Waiting for creator to start a new round!" }
            });
        }
        #endregion
    }
}
