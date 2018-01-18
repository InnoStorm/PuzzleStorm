using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client {

    /// <summary>
    /// ViewModel za main stranicu
    /// </summary>
    public class MainPageViewModel : BaseViewModel {

        #region Properties

        public List<RoomsPropsViewModel> RoomsItemsList { get; set; }

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

            InitRooms();
        }

        private void InitRooms()
        {
            RoomsItemsList = new List<RoomsPropsViewModel>()
            {
                new RoomsPropsViewModel()
                {
                    Name = "Test1",
                    By = "Ja",
                    Difficulty = "16",
                    MaxPlayers = "5",
                    Rounds = "2"
                },
                new RoomsPropsViewModel()
                {
                    Name = "Test2",
                    By = "Ti",
                    Difficulty = "25",
                    MaxPlayers = "4",
                    Rounds = "2"
                },
                new RoomsPropsViewModel()
                {
                    Name = "Test3",
                    By = "On",
                    Difficulty = "36",
                    MaxPlayers = "2",
                    Rounds = "1"
                }
            };
        }

        #endregion

        #region Metode


        #region TriTacke

        /// <summary>
        /// F-ja kad klikne dugme tri tacke ili na all rooms dugme
        /// </summary>
        public void TriTackeButton() {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new RoomsListPage();
        }

        #endregion

        #region LogOut

        private SignOutResponse LoginTask(SignOutRequest request) {
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

            Task<SignOutResponse> task = new Task<SignOutResponse>(() => LoginTask(request));
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
