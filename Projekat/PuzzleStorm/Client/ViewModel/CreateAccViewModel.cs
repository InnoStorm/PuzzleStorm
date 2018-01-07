using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client {

    /// <summary>
    /// ViewModel za create acc stranicu
    /// </summary>
    public class CreateAccViewModel : BaseViewModel {

        #region Properties

        public string UserName { get; set; }

        public string Email { get; set; }

        //public SecureString Password { get; set; }

        #endregion

        #region Commands

        public ICommand CreateAccCommand { get; set;  }

        public ICommand BackCommand { get; set; }

        #endregion

        #region Constuctors

        public CreateAccViewModel()
        {
            CreateAccCommand = new RelayCommandWithParameter(async (parameter) => await Create(parameter));
            BackCommand = new RelayCommand(BackToLoginPage);
        }

        #endregion

        #region Metods

        // Login f-ja
        public async Task Create(object parameter)
        {

            //SIMULACIJA KREIRANJA
            Task.Delay(500);

            //CREATE NA SERVERU
            //var user = this.UserName;
            //var email = this.Email;
            //var pass = ((PasswordBox) parameter).Password;
            // CREATE ( USER, EMAIL, PASS )
        }

        public void BackToLoginPage()
        {
            ((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Login;
            ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new LoginPage();
            //((MainWindow) Application.Current.MainWindow).MainFrame.Source = new System.Uri("Pages/LoginPage.xaml", UriKind.Relative);
        }
        #endregion
    }
}
