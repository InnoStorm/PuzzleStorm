using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Client.Helpers.Communication;
using Communicator;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;

namespace Client {

    public class GamePageViewModel : BaseViewModel {

        #region Properties

        public Button SelectedPiece { get; set; }

        public List<string> ListaSourceSlika { get; set; }
        public List<string> ListaShuffleSlika { get; set; }

        public List<GameWhoPlaysItemViewModel> PlayersItems { get; set; }
        public List<GameScoreItemViewModel> ScoreItems { get; set; }

        #endregion

        #region Commands

        public ICommand ClickPieceCommand { get; set; }

        public ICommand ClickGridCommand { get; set; }

        #endregion

        #region Constuctors

        public GamePageViewModel()
        {
            //ClickPieceCommand = new RelayCommand(() => CreateButton());
            ClickPieceCommand = new RelayCommandWithParameter((parameter) => SelectPiece(parameter));
            ClickGridCommand = new RelayCommandWithParameter((parameter) => GridPiece(parameter));

            ListaSourceSlika = new List<string>();
            ListaShuffleSlika = new List<string>();

            PrezumiSlike();

            PlayersItems = new List<GameWhoPlaysItemViewModel>()
            {
                new GameWhoPlaysItemViewModel()
                {
                    UserName = "Pera1",
                    OnTheMove = false
                },
                new GameWhoPlaysItemViewModel()
                {
                    UserName = "Pera2",
                    OnTheMove = true
                },
                new GameWhoPlaysItemViewModel()
                {
                    UserName = "Pera3",
                    OnTheMove = false
                }
            };
            ScoreItems = new List<GameScoreItemViewModel>()
            {
                new GameScoreItemViewModel()
                {
                    UserName = "Pera1",
                    Score = "15"
                },
                new GameScoreItemViewModel()
                {
                    UserName = "Pera2",
                    Score = "45"
                },
                new GameScoreItemViewModel()
                {
                    UserName = "Pera3",
                    Score = "30"
                }
            };

            //InitializingAsync();
        }

        #endregion

        #region Metods

        private void PrezumiSlike() {
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_001.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_002.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_003.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_004.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_005.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_006.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_007.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_008.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_009.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_010.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_011.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_012.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_013.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_014.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_015.jpg");
            ListaSourceSlika.Add("../Images/PomocnaSlagalica/image_part_016.jpg");

            foreach (string s in ListaSourceSlika)
                ListaShuffleSlika.Add(s);

            ListaShuffleSlika.Shuffle();
        }

        private async void InitializingAsync() {
            GameCurrentStatusRequest request = new GameCurrentStatusRequest() {
                RoomId = Player.Instance.RoomId,
                RequesterId = Player.Instance.Id
            };

            GameCurrentStatusResponse response = await ClientUtils.PerformRequestAsync(API.Instance.GameInitAsync, request,
                "Initializing..");

            if (response == null) return;

            ListaSourceSlika = response.PiecesPaths;

            foreach (string s in ListaSourceSlika)
                ListaShuffleSlika.Add(s);

            ListaShuffleSlika.Shuffle();

            foreach (var p in response.ListOfPlayers)
            {
                PlayersItems.Add(new GameWhoPlaysItemViewModel()
                {
                    UserName = p.Username,
                    OnTheMove = response.CurrentPlayerId == p.PlayerId
                });

                ScoreItems.Add(new GameScoreItemViewModel()
                {
                    UserName = p.Username,
                    Score = 0.ToString()
                });
            }
        }

        public void SelectPiece(object parameter)
        {
            SelectedPiece = ((Button) parameter);
        }


        private void GridPiece(object parameter)
        {
            if (SelectedPiece != null)
            {
                Image sourceSlikaParametar =
                    (Image) ((Button) parameter).Template.FindName("SourceSlika", ((Button) parameter));
                Image sourceSlikaSelected = (Image) SelectedPiece.Template.FindName("SourceSlika", SelectedPiece);

                //string asd = sourceSlikaParametar.Source.ToString();

                if (sourceSlikaParametar.Source.ToString().Equals(sourceSlikaSelected.Source.ToString()))
                {
                    SelectedPiece.Visibility = Visibility.Hidden;
                    ((Button) parameter).Opacity = 100;

                    SelectedPiece = null;
                }
            }
        }

        #endregion
    }
}
