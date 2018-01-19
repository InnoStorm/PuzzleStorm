using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DTOLibrary.Enums;
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

        private LoginResponse LoginTask(LoginRequest request)
        {
            try
            {
                return RabbitBus.Instance.Bus.Request<LoginRequest, LoginResponse>(request);
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    Status = OperationStatus.Exception,
                    Details = ex.Message
                };
            }
        }

        public async Task Login(object parameter) {

            LoginRequest myRequest = new LoginRequest() {
                Username = UserName,
                Password = ((PasswordBox)parameter).Password
            };

            Task<LoginResponse> task = new Task<LoginResponse>(() => LoginTask(myRequest));
            task.Start();

            //UI LOADING 
            var popup = new LoadingPopup() {
                Message = { Text = "Just a moment.." }
            };

            LoginResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args) {
                response = await task;
                args.Session.Close(false);
            });

            //LoginResponse response = await task;

            if (response.Status != OperationStatus.Exception)
            {
                if (response.Status == OperationStatus.Successfull) {
                    var sampleMessageDialog = new SampleMessageDialog {
                        Message = { Text = "Login Successfull!" }
                    };

                    await DialogHost.Show(sampleMessageDialog);

                    Player.Instance.Id = response.PlayerId;

                    ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
                }
                else {

                    var sampleMessageDialog = new SampleMessageDialog {
                        Message = { Text = "Login error!\n" + response.Details }
                    };

                    await DialogHost.Show(sampleMessageDialog);
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

        #endregion

        //Create button f-ja
        public void CreateButton()
        {
            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.CreateAccount;
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new CreateAccount();
        }
        #endregion
    }
}
