using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using Communicator;
using DTOLibrary.Broadcasts;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using MaterialDesignThemes.Wpf;
using StormCommonData;
using StormCommonData.Events;

namespace Client {

    /// <summary>
    /// ViewModel za main stranicu
    /// </summary>
    public class MainPageViewModel : BaseViewModel {

        #region Private

        #endregion

        #region Properties

        public ObservableCollection<RoomsPropsViewModel> RoomsItemsList { get; set; }

        public bool NoRoomLabel { get; set; } = true;

        public string WelcomeMessage { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Komanda koja vodi do sve sobe klikom na tri tacke (ALL ROOMS BUTTON TOO)
        /// </summary>
        public ICommand TriTackeCommand { get; set; }

        /// <summary>
        /// Komadna za log out iz main stranice
        /// </summary>
        public ICommand LogOutCommand { get; set; }

        /// <summary>
        /// Komanda koja otvara prozor za kreiranje nove sobe
        /// </summary>
        public ICommand CreateNewRoomCommand { get; set; }

        #endregion

        #region Constructors

        public MainPageViewModel()
        {
            TriTackeCommand = new RelayCommand(TriTackeButton);
            LogOutCommand = new RelayCommand(async () => await LogOutButtonAsync());

            CreateNewRoomCommand = new RelayCommand(CreateNewRoom);

            RoomsItemsList = new ObservableCollection<RoomsPropsViewModel>();

            if (ListRooms.Instance.RoomsItemsList.Count == 0)
            {
                Application.Current.Dispatcher.InvokeAsync(InitRooms);
            }
            //InitializingAsync();
            else
            {
                RoomsItemsList = ListRooms.Instance.RoomsItemsList;
                NoRoomLabel = false;
            }

            WelcomeMessage = "Welcome to PuzzleStorm, " + Player.Instance.UserName + "!";

            ((MainWindow) Application.Current.MainWindow).Title = "PuzzleStorm! Have fun " + Player.Instance.UserName + "!";

            //TODO DETECT transition
            //Demo
            ActivateTransition(WindowTransition.HomeEnter);
        }


        private void ActivateTransition(WindowTransition transition)
        {
            //Samo prelazi ToHome (To-this-view)
            switch (transition)
            {

                case WindowTransition.HomeEnter:
                    ClientUtils.SwitchState.HomeEnter();
                    ClientUtils.RoomChanged += OnRoomChange;
                    break;
                case WindowTransition.HomeExit:
                    ClientUtils.SwitchState.HomeExit();
                    ClientUtils.RoomChanged -= OnRoomChange;
                    break;
                case WindowTransition.LoginToHome:
                    ClientUtils.SwitchState.LoginToHome();
                    break;
                case WindowTransition.LobbyOwnerToHome:
                    ClientUtils.SwitchState.LobbyOwnerToHome();
                    break;
                case WindowTransition.LobbyJoinerToHome:
                    ClientUtils.SwitchState.LobbyJoinerToHome();
                    break;
                case WindowTransition.CreateRoomToHome:
                    ClientUtils.SwitchState.CreateRoomToHome();
                    break;
                case WindowTransition.HomeToHome:
                    break;
                default:
                    return;
            }

            //ClientUtils.RoomChanged += OnRoomChange;
            //ClientUtils.InRoomChange += OnInRoomChange;
        }

        #region InitRooms

        private async Task InitRooms()
        {
            GetAllRoomsRequest myRequest = new GetAllRoomsRequest() {
                   RequesterId = Player.Instance.Id
            };

            GetAllRoomsResponse response = await ClientUtils.PerformRequestAsync(API.Instance.GetAllRoomsAsync,
                myRequest, "Initializing..");
            if (response == null) return;

            foreach (RoomInfo room in response.List)
            {
                ListRooms.Instance.RoomsItemsList.Add(
                    new RoomsPropsViewModel()
                    {
                        RoomId = room.RoomId,
                        Name = "Room #" + room.RoomId,
                        By = room.CreatorUsername,
                        Difficulty = room.Difficulty.ToString(),
                        MaxPlayers = room.MaxPlayers.ToString(),
                        Rounds = room.NumberOfRounds.ToString(),
                        Visibility = Visibility.Visible,
                        Locked = !room.IsPublic
                    }
                );
            }

            RoomsItemsList = ListRooms.Instance.RoomsItemsList;

            if (RoomsItemsList.Count != 0)
                NoRoomLabel = false;
        }

        #endregion

        #endregion

        #region Metode

        //private void OnInRoomChange(object o, StormEventArgs<RoomPlayerUpdate> stormEventArgs)
        //{
        //    throw new NotImplementedException();
        //}

        private void OnRoomChange(object o, StormEventArgs<RoomsStateUpdate> stormEventArgs)
        {
            ClientUtils.UpdateGUI(() =>
            {
                var update = stormEventArgs.Data;

                switch (update.UpdateType)
                {
                    case RoomUpdateType.Created:
                    case RoomUpdateType.BecameAvailable:
                        RoomsItemsList.Add(new RoomsPropsViewModel()
                            {
                                By = update.Creator.Username,
                                RoomId = update.RoomId,
                                MaxPlayers = update.MaxPlayers.ToString(),
                                Difficulty = update.Level.ToString(),
                                Locked = !update.IsPublic,
                                Name = update.Creator.Username,
                                Rounds = update.NumberOfRounds.ToString(),
                            });
                        ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                        NoRoomLabel = false;

                        break;

                    case RoomUpdateType.Modified:
                        var room = RoomsItemsList.FirstOrDefault(x => x.RoomId == update.RoomId);
                        int i = RoomsItemsList.IndexOf(room);
                        RoomsItemsList[i] = new RoomsPropsViewModel()
                        {
                            By = update.Creator.Username,
                            RoomId = update.RoomId,
                            MaxPlayers = update.MaxPlayers.ToString(),
                            Difficulty = update.Level.ToString(),
                            Locked = !update.IsPublic,
                            Name = update.Creator.Username,
                            Rounds = update.NumberOfRounds.ToString()
                        };
                        break;

                    case RoomUpdateType.Deleted:
                    case RoomUpdateType.Started:
                    case RoomUpdateType.Filled:
                        if (RoomsItemsList.Any(x => x.RoomId == update.RoomId))
                        {
                            RoomsItemsList.Remove(RoomsItemsList.SingleOrDefault(x => x.RoomId == update.RoomId));
                            ListRooms.Instance.RoomsItemsList = RoomsItemsList;

                            if (RoomsItemsList.Count == 0)
                                NoRoomLabel = true;
                        }
                        break;
                    default:
                    break;    
                }
            });
        }

        #region TriTacke

        /// <summary>
        /// F-ja kad klikne dugme tri tacke ili na all rooms dugme
        /// </summary>
        public void TriTackeButton() {

            ActivateTransition(WindowTransition.HomeExit);
            
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new RoomsListPage();
        }

        #endregion

        #region LogOut
        /// <summary>
        /// Dugme za logout
        /// </summary>
        public async Task LogOutButtonAsync() {

            ActivateTransition(WindowTransition.HomeExit);
            
            SignOutRequest request = new SignOutRequest() {
                RequesterId = Player.Instance.Id
            };

            SignOutResponse response = await ClientUtils.PerformRequestAsync(API.Instance.SignOutAsync, request,
                "Signing out..");

            if (response == null) return;

            ((MainWindow) Application.Current.MainWindow).Title = "PuzzleStorm!";

            Player.Instance.Id = -1;

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LoginPage();
        }

        #endregion

        #region CreateNewRoom

        /// <summary>
        /// F-ja za novu sobu
        /// </summary>
        public void CreateNewRoom() {

            ActivateTransition(WindowTransition.HomeExit);
            
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new CreateRoomPage(); ;
        }

        #endregion

        #endregion

    }
}
