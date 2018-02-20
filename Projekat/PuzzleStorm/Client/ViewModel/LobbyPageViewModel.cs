using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using StormCommonData.Enums;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using Communicator;
using DTOLibrary.Broadcasts;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;
using StormCommonData.Events;

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

            JoinedPlayersItems = new ObservableCollection<LobbyJoinedPlayerViewModel>();
            InitPlayersAsync();

            ActivateTransition(WindowTransition.LobbyEnter);
        }

        #endregion

        #region Metods

        private void ActivateTransition(WindowTransition transition)
        {
            switch (transition)
            {
                case WindowTransition.LobbyEnter:
                    ClientUtils.SwitchState.LobbyEnter(Player.Instance.RoomId);
                    ClientUtils.RoomChanged += OnRoomChange;
                    ClientUtils.InRoomChange += OnInRoomChange;
                    break;
                case WindowTransition.LobbyExit:
                    ClientUtils.SwitchState.LobbyExit();
                    ClientUtils.RoomChanged -= OnRoomChange;
                    ClientUtils.InRoomChange -= OnInRoomChange;
                    break;
            }
        }

        private void OnRoomChange (object o, StormEventArgs<RoomsStateUpdate> stormEventArgs)
        {
            ClientUtils.UpdateGUI(() =>
            {
                var update = stormEventArgs.Data;

                switch (update.UpdateType)
                {
                    case RoomUpdateType.Modified:
                        RoomStatus = new RoomsPropsViewModel() {
                            MaxPlayers = update.MaxPlayers.ToString(),
                            Rounds = update.NumberOfRounds.ToString(),
                            Difficulty = MiniHelpFunctions.CastDifficulty(update.Level),
                            ChangeCommand = new RelayCommand(ChangeButtonAsync)
                        };
                        break;

                    case RoomUpdateType.Started:
                        ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new GamePage();
                        break;
                }
            });
        }

        private void OnInRoomChange(object o, StormEventArgs<RoomPlayerUpdate> stormEventArgs)
        {
            ClientUtils.UpdateGUI(() =>
            {
                var update = stormEventArgs.Data;

                switch (update.UpdateType)
                {
                    case RoomPlayerUpdateType.Joined:
                        if (update.Player.Username != Player.Instance.UserName)
                        {
                            JoinedPlayersItems.Add(new LobbyJoinedPlayerViewModel()
                            {
                                Username = update.Player.Username,
                                Ready = false
                            });
                        }
                        break;

                    case RoomPlayerUpdateType.LeftRoom:
                        JoinedPlayersItems.Remove(JoinedPlayersItems.Single(x => x.Username == update.Player.Username));
                        break;

                    case RoomPlayerUpdateType.ChangedStatus:
                        var player = JoinedPlayersItems.FirstOrDefault(x => x.Username == update.Player.Username);
                        int i = JoinedPlayersItems.IndexOf(player);
                        JoinedPlayersItems[i] = new LobbyJoinedPlayerViewModel()
                        {
                            Username = update.Player.Username,
                            Ready = update.Player.IsReady
                        };
                        break;
                }
            });
        }

        private async void ChangeButtonAsync() {

            if (Player.Instance.Creator)
            {

                var dialog = new ChangeRoomsPropsDialog()
                {
                    BoxDiff = {SelectedIndex = MiniHelpFunctions.DifficultyToIndex(RoomStatus.Difficulty)},
                    BoxMax = {SelectedIndex = Int32.Parse(RoomStatus.MaxPlayers) - 2},
                    BoxRnd = {SelectedIndex = Int32.Parse(RoomStatus.Rounds) - 1}
                };

                await DialogHost.Show(dialog, async delegate(object sender, DialogClosingEventArgs args)
                {

                    if (args.Parameter != null)
                    {

                        List<String> paramsList = (List<String>) args.Parameter;

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
            else
            {
                await DialogHost.Show(new SampleMessageDialog {
                    Message = { Text = "Ne posedujete privilegije!" }
                }
            );
            }
        }

        //Create button f-ja
        public async void StartReadyGameAsync() {
            //this.JoinedPlayersItems[0] = new LobbyJoinedPlayerViewModel() { Username = JoinedPlayersItems[0].Username, Ready = true};

            if (Player.Instance.Creator) {
                //PUSTA SE IGRICA

                StartRoomRequest request = new StartRoomRequest()
                {
                    RequesterId = Player.Instance.Id,
                    RoomId = Player.Instance.RoomId
                };

                var response = await ClientUtils.PerformRequestAsync(API.Instance.StartRoomAsync, request,
                    "Starting..");

                if (response == null) return;

                Player.Instance.CommKey = response.CreatedGame.CommunicationKey;

                ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new GamePage();
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

            if (Player.Instance.Creator)
                lobbyBy = Player.Instance.UserName;
            else
                lobbyBy = response.Creator.Username;
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
            Player.Instance.RoomId = -1;

            ActivateTransition(WindowTransition.LobbyExit);
            
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

            Player.Instance.RoomId = -1;

            ActivateTransition(WindowTransition.LobbyExit);
            
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        #endregion

        #endregion
    }
}
