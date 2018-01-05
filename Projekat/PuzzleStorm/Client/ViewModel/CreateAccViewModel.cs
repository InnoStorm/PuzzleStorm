using System.Security;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        #endregion

        #region Constuctors

        public CreateAccViewModel()
        {
            CreateAccCommand = new RelayCommandWithParameter(async (parameter) => await Create(parameter));
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

        #endregion
    }
}
