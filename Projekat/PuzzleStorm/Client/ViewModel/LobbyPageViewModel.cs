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

        #endregion

        #region Commands

        public ICommand StartReadyCommand { get; set; }

        public ICommand BackCommand { get; set; }


        #endregion

        #region Constuctors

        public LobbyPageViewModel()
        {
            StartReadyCommand = new RelayCommand(StartReadyGame);

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

            /*
            RoomStatus = new RoomsPropsViewModel() {
                MaxPlayers = 6.ToString(),
                Rounds = 6.ToString(),
                ChangeCommand = new RelayCommand(ChangeButton)
            };
            */


            var dialog = new ChangeRoomsPropsDialog() {
                BoxDiff = { SelectedIndex = MiniHelpFunctions.DifficultyToIndex(RoomStatus.Difficulty) },
                BoxMax = { SelectedIndex = Int32.Parse(RoomStatus.MaxPlayers) - 2 },
                BoxRnd = { SelectedIndex = Int32.Parse(RoomStatus.Rounds) - 1 }
            };

            await DialogHost.Show(dialog, delegate (object sender, DialogClosingEventArgs args)
            {
                
            });
        }

        //Create button f-ja
        public void StartReadyGame()
        {
            this.JoinedPlayersItems[0] = new LobbyJoinedPlayerViewModel() { Username = JoinedPlayersItems[0].Username, Ready = true};
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
