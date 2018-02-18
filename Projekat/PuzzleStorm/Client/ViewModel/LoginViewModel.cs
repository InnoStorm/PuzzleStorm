using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using Communicator;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;
using MaterialDesignThemes.Wpf;
using PropertyChanged;

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
            CreateAccButtonCommand = new RelayCommand(CreateButton);
        }

        #endregion

        #region Metods

        #region Login

        public async Task Login(object parameter) {

            LoginRequest myRequest = new LoginRequest() {
                Username = UserName,
                Password = ((PasswordBox)parameter).Password
            };
            
            LoginResponse response = await ClientUtils.PerformRequestAsync(API.Instance.LoginAsync, myRequest, "Just a moment..");
            if (response == null) return;
            
            Player.Instance.Id = response.PlayerId;
            Player.Instance.UserName = UserName;

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        #endregion

        public void ActivateTransition(WindowTransition transition)
        {
            if (transition == WindowTransition.HomeToLogin)
                ClientUtils.SwitchState.HomeToLogin();
        }
        
        //Create button f-ja
        public void CreateButton() {
            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.CreateAccount;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new CreateAccount();
        }
        #endregion
    }
}
