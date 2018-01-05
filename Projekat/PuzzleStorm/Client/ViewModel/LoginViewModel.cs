using System.Security;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        #endregion

        #region Constuctors

        public LoginViewModel()
        {
            LoginCommand = new RelayCommandWithParameter(async (parameter) => await Login(parameter));
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

        #endregion
    }
}
