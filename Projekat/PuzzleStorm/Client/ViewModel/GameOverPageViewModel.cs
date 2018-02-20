using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Eventing.Reader;
using System.Windows;
using System.Windows.Input;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using DTOLibrary.Broadcasts;
using MaterialDesignThemes.Wpf;
using StormCommonData.Enums;
using StormCommonData.Events;

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
            ActivateTransition(WindowTransition.GameOverEnter);

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
                BackOrStartCommand = new RelayCommand(WaitForRoundAsync);
                ButtonText = "NEW ROUND IN 5 SECONDS!";
            }
        }

        #endregion

        #region Metods

        private void ActivateTransition(WindowTransition transition)
        {
            switch (transition)
            {
                case WindowTransition.GameOverEnter:
                    ClientUtils.SwitchState.LobbyEnter(Player.Instance.RoomId);
                    ClientUtils.RoomChanged += OnRoomChange;
                    break;

                case WindowTransition.GameOverExit:
                    ClientUtils.SwitchState.LobbyExit();
                    ClientUtils.RoomChanged -= OnRoomChange;
                    break;
            }
        }

        private void OnRoomChange(object o, StormEventArgs<RoomsStateUpdate> stormEventArgs) {
            ClientUtils.UpdateGUI(() => {
                var update = stormEventArgs.Data;

                switch (update.UpdateType) {
                    
                    case RoomUpdateType.Continued:
                        ActivateTransition(WindowTransition.GameOverExit);

                        ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new GamePage();
                        break;
                }
            });
        }

        public void BackToMain()
        {
            ActivateTransition(WindowTransition.GameOverExit);

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        public async void WaitForRoundAsync() {
            
        }
        #endregion
    }
}
