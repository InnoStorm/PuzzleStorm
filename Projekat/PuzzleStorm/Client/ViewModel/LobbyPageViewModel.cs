using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Helpers.Communication;
using Communicator;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client
{
    public class LobbyPageViewModel : BaseViewModel
    {
        #region Properties

        //public List<LobbyJoinedPlayerViewModel> JoinedPlayersItems { get; set; }

        public RoomsPropsViewModel RoomStatus { get; set; }

        public ObservableCollection<LobbyJoinedPlayerViewModel> JoinedPlayersItems { get; set; }

        public string lobbyBy { get; set; }

        public string StartReadyLabel { get; set; }

        #endregion

        #region Commands

        public ICommand StartReadyCommand { get; set; }

        public ICommand BackCommand { get; set; }


        #endregion

        #region Constuctors

        public LobbyPageViewModel()
        {
            StartReadyCommand = new RelayCommand(StartReadyGameAsync);
            StartReadyLabel = Player.Instance.Creator ? "START" : "READY";

            if (Player.Instance.Creator)
                BackCommand = new RelayCommand(async () => await DeleteRoomAsync());
            else
                BackCommand = new RelayCommand(async () => await DisconnectFromRoomAsync());

            /*
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
            */

            JoinedPlayersItems = new ObservableCollection<LobbyJoinedPlayerViewModel>();
            InitPlayersAsync();

        }

        #endregion

        #region Metods

        private async void ChangeButtonAsync() {

            var dialog = new ChangeRoomsPropsDialog() {
                BoxDiff = { SelectedIndex = MiniHelpFunctions.DifficultyToIndex(RoomStatus.Difficulty) },
                BoxMax = { SelectedIndex = Int32.Parse(RoomStatus.MaxPlayers) - 2 },
                BoxRnd = { SelectedIndex = Int32.Parse(RoomStatus.Rounds) - 1 }
            };

            await DialogHost.Show(dialog, async delegate (object sender, DialogClosingEventArgs args) {

                if (args.Parameter != null)
                {

                    List<String> paramsList = (List<String>)args.Parameter;

                    ChangeRoomPropertiesRequest request = new ChangeRoomPropertiesRequest()
                    {
                        Difficulty = MiniHelpFunctions.StringToDifficulty(paramsList[0]),
                        MaxPlayers = Int32.Parse(paramsList[1]),
                        NumberOfRounds = Int32.Parse(paramsList[2]),
                        RoomId = Player.Instance.RoomId,
                        RequesterId = Player.Instance.Id
                    };

                    ChangeRoomPropertiesResponse response =
                        await ClientUtils.PerformRequestAsync(API.Instance.ChangeRoomPropertiesAsync, request, null);

                    if (response == null) return;

                    RoomStatus = new RoomsPropsViewModel()
                    {
                        MaxPlayers = response.MaxPlayers.ToString(),
                        Rounds = response.NumberOfRounds.ToString(),
                        Difficulty = MiniHelpFunctions.CastDifficulty(response.Difficulty),
                        ChangeCommand = new RelayCommand(ChangeButtonAsync)
                    };

                }
            });
        }

        //Create button f-ja
        public async void StartReadyGameAsync() {
            //this.JoinedPlayersItems[0] = new LobbyJoinedPlayerViewModel() { Username = JoinedPlayersItems[0].Username, Ready = true};

            if (Player.Instance.Creator) {
                //PUSTA SE IGRICA
            }
            else {

                if (StartReadyLabel == "READY") //SALJE DA JE READY
                {
                    ChangeStatusRequest request = new ChangeStatusRequest()
                    {
                        RequesterId = Player.Instance.Id,
                        IAmReady = true
                    };

                    ChangeStatusResponse response = await ClientUtils.PerformRequestAsync(
                        API.Instance.ChangeStatusAsync,
                        request, "Moment..");

                    if (response == null) return;

                    StartReadyLabel = "NOT READY";
                }
                else // SALJE DA NIJE READY
                {
                    ChangeStatusRequest request = new ChangeStatusRequest() {
                        RequesterId = Player.Instance.Id,
                        IAmReady = false
                    };

                    ChangeStatusResponse response = await ClientUtils.PerformRequestAsync(
                        API.Instance.ChangeStatusAsync,
                        request, "Moment..");

                    if (response == null) return;

                    StartReadyLabel = "READY";
                }
            }
        }

        #region InitPlayers

        private async Task InitPlayersAsync()
        {
            RoomCurrentStateRequest request = new RoomCurrentStateRequest()
            {
                RequesterId = Player.Instance.Id,
                RoomId = Player.Instance.RoomId
            };

            RoomCurrentStateResponse response = await ClientUtils.PerformRequestAsync(API.Instance.GetRoomCurrentStateAsync,
                request, "Initializing..");

            if (response == null) return;

            RoomStatus = new RoomsPropsViewModel()
            {
                Difficulty = MiniHelpFunctions.CastDifficulty(response.Difficulty),
                MaxPlayers = response.MaxPlayers.ToString(),
                Rounds = response.NumberOfRounds.ToString(),
                ChangeCommand = new RelayCommand(ChangeButtonAsync)
            };

            foreach (DTOLibrary.SubDTOs.Player player in response.Players) {
                JoinedPlayersItems.Add(
                    new LobbyJoinedPlayerViewModel() {
                        Username = player.Username,
                        Ready = player.IsReady
                    }
                );
            }

            if (Player.Instance.Creator) lobbyBy = Player.Instance.UserName;
        }

        #endregion

        #region DeleteRoom

        private async Task DeleteRoomAsync()
        {

            CancelRoomRequest request = new CancelRoomRequest()
            {
                RequesterId = Player.Instance.Id,
                RoomId = Player.Instance.RoomId
            };

            CancelRoomResponse response = await ClientUtils.PerformRequestAsync(API.Instance.CancelRoomAsync, request,
                "Deleting a room..");

            if (response == null) return;

            var sampleMessageDialog = new SampleMessageDialog {
                Message = { Text = "Room deleted Successfull!" }
            };

            await DialogHost.Show(sampleMessageDialog);

            Player.Instance.Creator = false; //vise nije creator sobe

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }
        #endregion

        #region Disconnect

        private async Task DisconnectFromRoomAsync() {

            LeaveRoomRequest request = new LeaveRoomRequest() {
                RequesterId = Player.Instance.Id,
                RoomId = Player.Instance.RoomId
            };

            LeaveRoomResponse response = await ClientUtils.PerformRequestAsync(API.Instance.LeaveRoomAsync, request,
                "Leaving.. :(");

            if (response == null) return;

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        #endregion

        #endregion
    }
}
