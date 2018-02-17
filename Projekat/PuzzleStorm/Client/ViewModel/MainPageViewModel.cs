using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Helpers.Communication;
using Communicator;
using DTOLibrary.Broadcasts;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using MaterialDesignThemes.Wpf;
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
                Application.Current.Dispatcher.InvokeAsync(InitRooms);
                //InitializingAsync();
            else
                RoomsItemsList = ListRooms.Instance.RoomsItemsList;

            NoRoomLabel = false;

            //ClientUtils.RoomChanged += OnRoomChange;
            //ClientUtils.InRoomChange += OnInRoomChange;

            ClientUtils.RoomChanged += (sender, args) =>
            {
                Console.WriteLine("PALIS LI ME?");
            };

            
            ClientUtils.SwitchState.LoginToHome();
            //Application.Current.Dispatcher.InvokeAsync(ClientUtils.SwitchState.LoginToHome);
            //different 
        }

        
        #region InitRooms

        private async Task InitRooms()
        {
            GetAllRoomsRequest myRequest = new GetAllRoomsRequest() {
                   RequesterId = Player.Instance.Id
            };

            var response = await ClientUtils.PerformRequestAsync(API.Instance.GetAllRoomsAsync, myRequest);
            if (response == null) return;

            foreach (RoomInfo room in response.List)
            {
                ListRooms.Instance.RoomsItemsList.Add(
                    new RoomsPropsViewModel()
                    {
                        RoomId = room.RoomId,
                        Name = "Room #" + room.RoomId,
                        By = room.CreatorUsername,
                        Difficulty = CastDifficulty(room.Difficulty),
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

        private string CastDifficulty(PuzzleDifficulty level) {
            switch (level)
            {
                case PuzzleDifficulty.Easy:
                    return 16.ToString();
                case PuzzleDifficulty.Medium:
                    return 25.ToString();
                case PuzzleDifficulty.Hard:
                    return 36.ToString();
                default:
                    return "X";
            }
        }

        #endregion

        #endregion

        #region Metode


        //TODO REPLACE
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

        private void OnInRoomChange(object o, StormEventArgs<RoomPlayerUpdate> stormEventArgs)
        {
            throw new NotImplementedException();
        }

        private void OnRoomChange(object o, StormEventArgs<RoomsStateUpdate> stormEventArgs)
        {
            var update = stormEventArgs.Data;

            switch (update.UpdateType)
            {
                case RoomUpdateType.Created:
                case RoomUpdateType.BecameAvailable:
                    ClientUtils.UpdateGUI(() =>
                    {
                        RoomsItemsList.Add(new RoomsPropsViewModel()
                        {
                            By = update.Creator.Username,
                            RoomId = update.RoomId,
                            MaxPlayers = update.MaxPlayers.ToString(),
                            Difficulty = CastDifficulty(update.Level),
                            Locked = !update.IsPublic,
                            Name = update.Creator.Username,
                            Rounds = update.NumberOfRounds.ToString(),
                        });

                        ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                    });
                    break;

                case RoomUpdateType.Modified:
                    break;

                case RoomUpdateType.Deleted:
                case RoomUpdateType.Started:
                case RoomUpdateType.Filled:
                    ClientUtils.UpdateGUI(() =>
                    {
                        RoomsItemsList.Remove(RoomsItemsList.SingleOrDefault(x => x.RoomId == update.RoomId));
                        ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

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

        private SignOutResponse LogoutTask(SignOutRequest request) {
            try {
                return RabbitBus.Instance.Bus.Request<SignOutRequest, SignOutResponse>(request);
            }
            catch (Exception ex) {
                return new SignOutResponse() {
                    Status = OperationStatus.Exception,
                    Details = ex.InnerException.Message
                };
            }
        }

        /// <summary>
        /// Dugme za logout
        /// </summary>
        public async Task LogOutButtonAsync() {

            SignOutRequest request = new SignOutRequest() {
                RequesterId = Player.Instance.Id
            };

            Task<SignOutResponse> task = new Task<SignOutResponse>(() => LogoutTask(request));
            task.Start();

            //UI LOADING 
            var popup = new LoadingPopup() {
                Message = { Text = "Signing out.." }
            };

            SignOutResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args) {
                response = await task;
                args.Session.Close(false);
            });

            if (response.Status != OperationStatus.Exception)
            {
                if (response.Status == OperationStatus.Successfull)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = {Text = "Sign out Successfull!" }
                    };

                    await DialogHost.Show(sampleMessageDialog);
                }
                else
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = {Text = "Sigh out error!\n" + response.Details}
                    };

                    await DialogHost.Show(sampleMessageDialog);
                }

                ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LoginPage();
            }
            else
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = {Text = "Exception: " + response.Details}
                };

                await DialogHost.Show(sampleMessageDialog);
            }
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
