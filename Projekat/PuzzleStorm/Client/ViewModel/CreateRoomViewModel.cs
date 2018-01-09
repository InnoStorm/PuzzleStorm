using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace Client {
    public class CreateRoomViewModel : BaseViewModel {

        #region Properties

        public int Difficulty { get; set; } = 8;

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
            CreateRoomCommand = new RelayCommandWithParameter(async (parameter) => { await CreateRoom(parameter); });
            CancelCommand = new RelayCommand(BackToMainPage);

            #region Inicijalizacija Combobox-ova

            //Combobox za diff
            CmbDifficulty = new ObservableCollection<int>
            {
                8,
                16,
                32
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

        // Login f-ja
        public async Task CreateRoom(object parameter) {

            //SIMULACIJA KREIRANJA
            Task.Delay(500);

            var sampleMessageDialog = new SampleMessageDialog {
                Message = { Text = "Uspesno ste kreirali sobu!" }
            };

            await DialogHost.Show(sampleMessageDialog);

            //ZOVE SE F-ja NA SERVERU
            //CREATEROOM ( Difficulty, MaxPlayers, Rounds, Password)

            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LobbyPage();
        }

        public void BackToMainPage() {

            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.MainPage;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
            //((MainWindow) Application.Current.MainWindow).MainFrame.Source = new System.Uri("Pages/LoginPage.xaml", UriKind.Relative);

        }
        #endregion
    }
}
