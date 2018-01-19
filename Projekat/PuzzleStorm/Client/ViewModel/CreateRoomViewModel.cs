using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;

namespace Client {
    public class CreateRoomViewModel : BaseViewModel {

        #region Properties

        public int Difficulty { get; set; } = 16;

        public int MaxPlayers { get; set; } = 2;

        public int Rounds { get; set; } = 1;

        public string Password { get; set; }

        // 3 combobox-a
        public ObservableCollection<int> CmbDifficulty { get; private set; }
        public ObservableCollection<int> CmbMaxPlayers { get; private set; }
        public ObservableCollection<int> CmbRounds { get; private set; }

        //public SecureString Password { get; set; }

        #endregion

        #region Commands

        public ICommand CreateRoomCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion

        #region Constuctors

        public CreateRoomViewModel() {
            CreateRoomCommand = new RelayCommand(async () => await CreateRoom());
            CancelCommand = new RelayCommand(BackToMainPage);

            #region Inicijalizacija Combobox-ova

            //Combobox za diff
            CmbDifficulty = new ObservableCollection<int>
            {
                16,
                25,
                36
            };

            //Combobox za max
            CmbMaxPlayers = new ObservableCollection<int>
            {
                2,
                3,
                4,
                5,
                6
            };

            //Combobox za runde
            CmbRounds = new ObservableCollection<int>
            {
                1,
                2,
                3,
                4,
                5,
            };

            #endregion
        }

        #endregion

        #region Metods

        #region Create

        private CreateRoomResponse CreateTask(CreateRoomRequest request) {
            try {
                return RabbitBus.Instance.Bus.Request<CreateRoomRequest, CreateRoomResponse>(request);
            }
            catch (Exception ex) {
                return new CreateRoomResponse() {
                    Status = OperationStatus.Exception,
                    Details = ex.Message
                };
            }
        }

        public async Task CreateRoom()
        {

            // Napravi se request
            CreateRoomRequest request = new CreateRoomRequest()
            {
                RequesterId = Player.Instance.Id,
                MaxPlayers = MaxPlayers,
                NumberOfRounds = Rounds,
                Password = Password
            };
            // Dificulty za request
            switch (Difficulty)
            {
                case 16:
                    request.Level = PuzzleDifficulty.Begginer;
                    break;

                case 25:
                    request.Level = PuzzleDifficulty.Intermediate;
                    break;

                case 36:
                    request.Level = PuzzleDifficulty.Advanced;
                    break;
            }

            // Taks za create
            Task<CreateRoomResponse> task = new Task<CreateRoomResponse>(() => CreateTask(request));
            task.Start();

            // UI
            var popup = new LoadingPopup() {
                Message = { Text = "Creating a room.." }
            };

            CreateRoomResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args) {
                response = await task;
                args.Session.Close(false);
            });

            if (response.Status != OperationStatus.Exception) {
                if (response.Status == OperationStatus.Successfull) {
                    var sampleMessageDialog = new SampleMessageDialog {
                        Message = { Text = "Room created successfully!" }
                    };

                    await DialogHost.Show(sampleMessageDialog);

                    Player.Instance.RoomId = response.RoomId;

                    ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LobbyPage();
                }
                else {

                    var sampleMessageDialog = new SampleMessageDialog {
                        Message = { Text = "Room creating error!\n" + response.Details }
                    };

                    await DialogHost.Show(sampleMessageDialog);
                }
            }
            else
            {
                var sampleMessageDialog = new SampleMessageDialog {
                    Message = { Text = "Exception: " + response.Details }
                };

                await DialogHost.Show(sampleMessageDialog);
            }
        }

        #endregion

        public void BackToMainPage() {

            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.MainPage;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
            //((MainWindow) Application.Current.MainWindow).MainFrame.Source = new System.Uri("Pages/LoginPage.xaml", UriKind.Relative);

        }
        #endregion
    }
}
