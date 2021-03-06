﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using Communicator;
using DTOLibrary.Broadcasts;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using EasyNetQ;
using MaterialDesignThemes.Wpf;
using StormCommonData.Enums;
using StormCommonData.Events;

namespace Client {

    public class GamePageViewModel : BaseViewModel {

        #region Properties

        public Button SelectedPiece { get; set; }

        public ObservableCollection<string> ListaSlika { get; set; }

        public ObservableCollection<string> ListaSourceSlika { get; set; }
        public ObservableCollection<string> ListaShuffleSlika { get; set; }

        public Button Proba { get; set; }

        //public List<string> ListaSourceSlika { get; set; }
        //public List<string> ListaShuffleSlika { get; set; }

        //public List<GameWhoPlaysItemViewModel> PlayersItems { get; set; }
        //public List<GameScoreItemViewModel> ScoreItems { get; set; }

        public ObservableCollection<GameWhoPlaysItemViewModel> PlayersItems { get; set; }
        public ObservableCollection<GameScoreItemViewModel> ScoreItems { get; set; }

        #endregion

        #region Commands

        public ICommand ClickPieceCommand { get; set; }

        public ICommand ClickGridCommand { get; set; }

        #endregion

        #region Constuctors

        public GamePageViewModel()
        {
            ClickPieceCommand = new RelayCommandWithParameter((parameter) => SelectPiece(parameter));
            ClickGridCommand = new RelayCommandWithParameter((parameter) => GridPiece(parameter));

            ListaSourceSlika = new ObservableCollection<string>();
            ListaShuffleSlika = new ObservableCollection<string>();

            ListaSlika = new ObservableCollection<string>();

            //PrezumiSlike();

            PlayersItems = new ObservableCollection<GameWhoPlaysItemViewModel>();
            ScoreItems = new ObservableCollection<GameScoreItemViewModel>();

            InitializingAsync();

            ActivateTransition(WindowTransition.GameEnter);
        }

        #endregion

        #region Metods

        private void ActivateTransition(WindowTransition transition)
        {
            switch (transition)
            {
                case WindowTransition.GameEnter:
                    ClientUtils.SwitchState.GameEnter();
                    ClientUtils.SwitchState.LobbyEnter(Player.Instance.RoomId);
                    ClientUtils.RoomChanged += OnRoomChange;
                    ClientUtils.GameUpdated += GameChanged;
                    break;

                case WindowTransition.GameExit:
                    ClientUtils.SwitchState.GameExit();
                    ClientUtils.SwitchState.LobbyExit();
                    ClientUtils.RoomChanged -= OnRoomChange;
                    ClientUtils.GameUpdated -= GameChanged;
                    break;
            }
        }

        private void OnRoomChange(object o, StormEventArgs<RoomsStateUpdate> stormEventArgs) {
            ClientUtils.UpdateGUI(async () => {

                await DialogHost.Show(new SampleMessageDialog {
                    Message = { Text = "Room has been destroyed!" }
                });

                ActivateTransition(WindowTransition.GameExit);

                Player.Instance.CommKey = null;
                Player.Instance.OnTheMove = false;
                Player.Instance.RoomId = -1;
                Player.Instance.Creator = false;

                ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();

            });
        }

        private void GameChanged(object sender, StormEventArgs<GameUpdate> stormEventArgs)
        {
            ClientUtils.UpdateGUI(() =>
            {
                var update = stormEventArgs.Data;

                switch (update.UpdateType)
                {
                    case GamePlayUpdateType.Playing:
                       
                        //obradi potez
                        if (update.PlayedMove.IsSuccessfull)
                            OdigrajPotez(update.PlayedMove);
                        else
                            PrikaziPotez(update.PlayedMove);

                        //promeni ko igra
                        PromeniTrenutnogIgraca(update.CurrentPlayer);

                        //update-uj score
                        PromeniScore(update.Scoreboard);
                        
                        break;

                    case GamePlayUpdateType.RoundOver:

                        //update-uj score
                        PromeniScore(update.Scoreboard);

                        //prebaci se stranicu za prikaz bodova 
                        ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new GameOverPage();
                        ActivateTransition(WindowTransition.GameExit);
                        break;

                    case GamePlayUpdateType.GameOver:

                        //update-uj score
                        PromeniScore(update.Scoreboard);

                        Player.Instance.InGame.Over = true;
                        //prebaci se na poslednju stranicu
                        ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new GameOverPage();
                        ActivateTransition(WindowTransition.GameExit);
                        break;
                }
            });
        }

        private void PrezumiSlike() {
            ListaSlika.Add("../Images/1/16/image_part_001.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_002.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_003.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_004.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_005.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_006.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_007.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_008.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_009.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_010.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_011.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_012.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_013.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_014.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_015.jpg");
            ListaSlika.Add("../Images/PomocnaSlagalica/image_part_016.jpg");

            foreach (string s in ListaSlika)
                ListaShuffleSlika.Add(s);

            for(int i = 0; i < ListaSlika.Count; ++i)
                ListaSourceSlika.Add(null);

            ListaShuffleSlika.Shuffle();
        }
        
        private async void InitializingAsync() {
            
            LoadGameRequest request = new LoadGameRequest()
            {
                RoomId = Player.Instance.RoomId,
                RequesterId = Player.Instance.Id
            };

            API.Instance.Send(request, Player.Instance.CommKey);
            API.Instance.ReceiveLoadGameResponse(Player.Instance.Id, LoadGameResponseHandler);
        }
        
        private void LoadGameResponseHandler(LoadGameResponse response)
        {
            //check if response.Status == OperationStatus.Failed
            
            ClientUtils.UpdateGUI(() =>
            {
                // preuzmu se slike u ListuSlika
                foreach (var pic in response.PiecesPaths) {
                    ListaSlika.Add(pic);
                }

                // prebace se u shuffle listu
                foreach (string s in ListaSlika)
                    ListaShuffleSlika.Add(s);

                ListaShuffleSlika.Shuffle();

                // napravi se mesta u listu source slika
                for (int i = 0; i < ListaSlika.Count; ++i)
                    ListaSourceSlika.Add("../Images/qm2.png");

                //PrezumiSlike();

                //preuzimanje igraca
                foreach (var p in response.ListOfPlayers) {
                    PlayersItems.Add(new GameWhoPlaysItemViewModel() {
                        UserName = p.Username,
                        OnTheMove = response.CurrentPlayerId == p.PlayerId
                    });

                    if (response.CurrentPlayerId == Player.Instance.Id)
                        Player.Instance.OnTheMove = true;
                }

                //preuzimanje scoreboarda
                foreach (var s in response.ScoreBoard.Scores) {
                    ScoreItems.Add(new GameScoreItemViewModel() {
                        UserName = s.Item1.Username,
                        Score = s.Item2.ToString()
                    });
                }

                // sacuva se score globalno
                Player.Instance.InGame.Score = response.ScoreBoard;
                Player.Instance.InGame.Over = false;
            });
        }

        public async void SelectPiece(object parameter)
        {
            if (Player.Instance.OnTheMove)
            {
                if (SelectedPiece != null)
                    ((Grid) SelectedPiece.Template.FindName("bg", SelectedPiece)).Background = Brushes.Transparent;

                SelectedPiece = ((Button) parameter);

                Image sourceSlikaSelected = (Image)SelectedPiece.Template.FindName("SourceSlika", SelectedPiece);

                string selected = sourceSlikaSelected.Source.ToString();

                if (!selected.Contains("part"))
                    SelectedPiece = null;
                else { 
                    Grid g = (Grid) (SelectedPiece.Template.FindName("bg", SelectedPiece));
                    g.Background = Brushes.DarkRed;
                }
            }
            else
            {
                await DialogHost.Show(new SampleMessageDialog
                {
                    Message = {Text = "Niste na potezu!"}
                });
            }
        }

        private async void GridPiece(object parameter)
        {
            if (Player.Instance.OnTheMove)
            {
                if (SelectedPiece != null)
                {
                    int indexTo = Int32.Parse((string) ((Button) parameter).Tag);

                    Image sourceSlikaSelected = (Image) SelectedPiece.Template.FindName("SourceSlika", SelectedPiece);

                    string selected = sourceSlikaSelected.Source.ToString();

                    if (!selected.Contains("part"))
                    {
                        SelectedPiece = null;
                        return;
                    }

                    int indexFrom = Int32.Parse(selected.Substring(selected.Length - 6, 2)) - 1;

                    MakeAMoveRequest request = new MakeAMoveRequest()
                    {
                        MoveToPlay = new Move()
                        {
                            PositionFrom = indexFrom,
                            PositionTo = indexTo
                        },
                        RequesterId = Player.Instance.Id,
                        RoomId = Player.Instance.RoomId
                    };

                    //SEND REQUEST
                    API.Instance.Send(request, Player.Instance.CommKey);

                    Player.Instance.OnTheMove = false;
                }
            }
            else {
                await DialogHost.Show(new SampleMessageDialog {
                    Message = { Text = "Niste na potezu!" }
                });
            }
        }

        #region Funkcije za promenu stanja

        private void PromeniTrenutnogIgraca(DTOLibrary.SubDTOs.Player currentPlayer)
        {
            var player = PlayersItems.FirstOrDefault(x => x.UserName == currentPlayer.Username);
            if (player != null && !player.OnTheMove) //ako je taj isti onda nista
            {
                //Player.Instance.OnTheMove = Player.Instance.Id == currentPlayer.PlayerId;
                
                //skinemo starog da ne igra
                var stari = PlayersItems.FirstOrDefault(x => x.OnTheMove);
                int i = PlayersItems.IndexOf(stari);

                if (stari != null)
                    PlayersItems[i] = new GameWhoPlaysItemViewModel() {
                        OnTheMove = false,
                        UserName = stari.UserName
                    };

                //setujemo novog da igra
                i = PlayersItems.IndexOf(player);
                PlayersItems[i] = new GameWhoPlaysItemViewModel() {
                    OnTheMove = true,
                    UserName = currentPlayer.Username
                };
            }

            if (currentPlayer.PlayerId == Player.Instance.Id)
                Player.Instance.OnTheMove = true;
        }

        private void OdigrajPotez(Move potez)
        {
            string slika = ListaSlika[potez.PositionFrom];

            //unesemo je u matricu slika
            ListaSourceSlika[potez.PositionFrom] = slika;

            //nadjemo je u shuffle i izbacimo
            int i = ListaShuffleSlika.IndexOf(slika);
            ListaShuffleSlika[i] = "../Images/qm2.png";

            if (Player.Instance.OnTheMove)
                if (SelectedPiece != null)
                    SelectedPiece.Opacity = 0;
        }

        private async void PrikaziPotez(Move potez)
        {
            if (SelectedPiece != null)
                ((Grid)SelectedPiece.Template.FindName("bg", SelectedPiece)).Background = Brushes.Transparent;
            
            string fromSlika = ListaSlika[potez.PositionFrom];
            
            ListaSourceSlika[potez.PositionTo] = fromSlika;

            int indexSh = ListaShuffleSlika.IndexOf(fromSlika);
            ListaShuffleSlika[indexSh] = "../Images/qm2.png";

            await Task.Delay(1000);

            ListaSourceSlika[potez.PositionTo] = "../Images/qm2.png";
            ListaShuffleSlika[indexSh] = fromSlika;

            if (SelectedPiece != null)
                ((Grid)SelectedPiece.Template.FindName("bg", SelectedPiece)).Background = Brushes.DarkRed;
        }


        private void PromeniScore(Scoreboard scoreboard) {

            ScoreItems.Clear();

            foreach (var s in scoreboard.Scores) {
                ScoreItems.Add(new GameScoreItemViewModel() {
                    UserName = s.Item1.Username,
                    Score = s.Item2.ToString()
                });
            }

            Player.Instance.InGame.Score = scoreboard;
        }

        #endregion

        #endregion
    }
}
