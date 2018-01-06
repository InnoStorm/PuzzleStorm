using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client {

    /// <summary>
    /// ViewModel za login stranicu
    /// </summary>
    public class LoginViewModel : BaseViewModel {

        #region Properties

        public string UserName { get; set; }

        //public SecureString Password { get; set; }

        #endregion

        #region Commands

        public ICommand LoginCommand { get; set;  }

        public ICommand CreateAccButtonCommand { get; set; }

        #endregion

        #region Constuctors

        public LoginViewModel()
        {
            LoginCommand = new RelayCommandWithParameter(async (parameter) => await Login(parameter));
            CreateAccButtonCommand = new RelayCommand(async () => await CreateButton());
        }

        #endregion

        #region Metods

        // Login f-ja
        public async Task Login(object parameter)
        {

            // SIMULACIJA LOGOVANJA
            Task.Delay(500);

            // LOGIN NA SERVERU 
            //var user = this.UserName;
            //var pass = ((PasswordBox) parameter).Password;
            // LOGIN_SERVER ( USER, PASS )
        }


        //Create button f-ja
        public async Task CreateButton()
        {
            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.CreateAccount;
            ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new CreateAccount();
            Task.Delay(500);
        }
        #endregion
    }
}
