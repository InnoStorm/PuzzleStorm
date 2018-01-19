using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using MaterialDesignThemes.Wpf;

namespace Client {

    /// <summary>
    /// ViewModel za main stranicu
    /// </summary>
    public class MainPageViewModel : BaseViewModel {

        #region Private

        private Task InitTask;

        #endregion

        #region Properties

        public List<RoomsPropsViewModel> RoomsItemsList { get; set; }

        public List<bool> VisibilityForMainRooms { get; set; }

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

            if (ListRooms.Instance.RoomsItemsList.Count == 0)
            {
                RoomsItemsList = new List<RoomsPropsViewModel>();
                NoRoomLabel = true;
                InitializingAsync();

                VisibilityForMainRooms = new List<bool>() {false, false, false};
            }
            else
            {
                RoomsItemsList = ListRooms.Instance.RoomsItemsList;
                NoRoomLabel = false;

                FillVisibilityForRooms();
            }
        }

        private async Task InitializingAsync() {
            await InitRooms();
        }


        #region InitRooms

        private GetAllRoomsResponse TakeRoomsTask(GetAllRoomsRequest request) {
            try {
                return RabbitBus.Instance.Bus.Request<GetAllRoomsRequest, GetAllRoomsResponse>(request);
            }
            catch (Exception ex) {
                return new GetAllRoomsResponse() {
                    Status = OperationStatus.Exception,
                    Details = ex.Message
                };
            }
        }

        private async Task InitRooms()
        {
            GetAllRoomsRequest myRequest = new GetAllRoomsRequest() {
                   RequesterId = Player.Instance.Id
            };

            Task<GetAllRoomsResponse> task = new Task<GetAllRoomsResponse>(() => TakeRoomsTask(myRequest));
            task.Start();

            //UI LOADING 
            var popup = new LoadingPopup() {
                Message = { Text = "Initializing.." }
            };

            GetAllRoomsResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args) {
                response = await task;
                args.Session.Close(false);
            });

            if (response.Status != OperationStatus.Exception) {
                if (response.Status == OperationStatus.Successfull) {

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
                                    Difficulty = CastDifficulty(room.Level),
                                    MaxPlayers = room.MaxPlayers.ToString(),
                                    Rounds = room.NumberOfRounds.ToString()
                                }
                            );
                        }

                        RoomsItemsList = ListRooms.Instance.RoomsItemsList;
                        NoRoomLabel = false;
                    }

                }
                else {
                    
                }
            }
            else //DOSO EXCEPTION
            {
                var sampleMessageDialog = new SampleMessageDialog {
                    Message = { Text = "Exception: " + response.Details }
                };

                await DialogHost.Show(sampleMessageDialog);
            }
        }

        private string CastDifficulty(PuzzleDifficulty level) {
            switch (level)
            {
                case PuzzleDifficulty.Begginer:
                    return 16.ToString();
                case PuzzleDifficulty.Intermediate:
                    return 25.ToString();
                case PuzzleDifficulty.Advanced:
                    return 36.ToString();
                default:
                    return "X";
            }
        }

        #endregion

        #endregion

        #region Metode

        // da se refaktorise!
        private void FillVisibilityForRooms() {

            VisibilityForMainRooms = new List<bool>(3);

            switch (RoomsItemsList.Count)
            {
                case 1:
                    VisibilityForMainRooms.Add(true);
                    VisibilityForMainRooms.Add(false);
                    VisibilityForMainRooms.Add(false);
                    break;

                case 2:
                    VisibilityForMainRooms.Add(true);
                    VisibilityForMainRooms.Add(true);
                    VisibilityForMainRooms.Add(false);
                    break;

                case 3:
                    VisibilityForMainRooms.Add(true);
                    VisibilityForMainRooms.Add(true);
                    VisibilityForMainRooms.Add(true);
                    break;
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
