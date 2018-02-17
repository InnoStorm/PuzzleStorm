using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Helpers.Communication;
using Communicator;
using DTOLibrary.Broadcasts;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using MaterialDesignThemes.Wpf;
using StormCommonData.Enums;

namespace Client {

    /// <summary>
    /// ViewModel za main stranicu
    /// </summary>
    public class MainPageViewModel : BaseViewModel {

        #region Private

        private Task InitTask;

        #endregion

        #region Properties

        public ObservableCollection<RoomsPropsViewModel> RoomsItemsList { get; set; }

        public bool NoRoomLabel { get; set; } = true;

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
                InitializingAsync();
            else
                RoomsItemsList = ListRooms.Instance.RoomsItemsList;

            NoRoomLabel = false;

            Subscribe();
        }

        private async Task InitializingAsync() {
            await InitRooms();
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

            if (response.List.Count != 0)
            {

                foreach (RoomInfo room in response.List)
                {
                    ListRooms.Instance.RoomsItemsList.Add(
                        new RoomsPropsViewModel()
                        {
                            RoomId = room.RoomId,
                            Name = "Room #" + room.RoomId,
                            By = room.CreatorUsername,
                            Difficulty = MiniHelpFunctions.CastDifficulty(room.Difficulty),
                            MaxPlayers = room.MaxPlayers.ToString(),
                            Rounds = room.NumberOfRounds.ToString(),
                            Visibility = Visibility.Visible,
                            Locked = !room.IsPublic
                        }
                    );
                }

                RoomsItemsList = ListRooms.Instance.RoomsItemsList;
                NoRoomLabel = false;
            }
        }

        #endregion

        #endregion

        #region Metode

        private void Subscribe() {
            var subResult = RabbitBus.Instance.Bus.SubscribeAsync<RoomsStateUpdate>("cl_" + Player.Instance.Id,
                    request =>
                        Task.Factory.StartNew(() => {
                            if (request.UpdateType == RoomUpdateType.Deleted) {
                                //var rem = RoomsItemsList.Where(x => x.RoomId == request.RoomId);
                                //RoomsItemsList.Remove(rem);

                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {

                                    RoomsPropsViewModel rem = null;

                                    foreach (var v in RoomsItemsList) {
                                        if (v.RoomId == request.RoomId)
                                            rem = v;
                                    }

                                    if (rem != null)
                                        RoomsItemsList.Remove(rem);

                                    ListRooms.Instance.RoomsItemsList = RoomsItemsList;

                                    if (RoomsItemsList.Count == 0) NoRoomLabel = true;
                                }));
                            }
                            else if (request.UpdateType == RoomUpdateType.Created) {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {
                                    RoomsItemsList.Add(new RoomsPropsViewModel() {
                                        By = request.Creator.Username,
                                        RoomId = request.RoomId
                                    });

                                    ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                                }));
                            }
                        }),
                    x => x.WithTopic("#"));
        }

        #region TriTacke

        /// <summary>
        /// F-ja kad klikne dugme tri tacke ili na all rooms dugme
        /// </summary>
        public void TriTackeButton() {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new RoomsListPage();
        }

        #endregion

        #region LogOut

        /// <summary>
        /// Dugme za logout
        /// </summary>
        public async Task LogOutButtonAsync() {

            SignOutRequest request = new SignOutRequest() {
                RequesterId = Player.Instance.Id
            };

            SignOutResponse response = await ClientUtils.PerformRequestAsync(API.Instance.SignOutAsync, request,
                "Signing out..");

            if (response == null) return;

            var sampleMessageDialog = new SampleMessageDialog {
                Message = { Text = "Sign out Successfull!" }
            };

            await DialogHost.Show(sampleMessageDialog);

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LoginPage();
        }

        #endregion

        #region CreateNewRoom

        /// <summary>
        /// F-ja za novu sobu
        /// </summary>
        public void CreateNewRoom() {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new CreateRoomPage(); ;
        }

        #endregion

        #endregion

    }
}
