using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;
using MaterialDesignThemes.Wpf;

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
            try {
                using (
                    var bus =
                        RabbitHutch.CreateBus(
                            "amqp://ygunknwy:pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ@sheep.rmq.cloudamqp.com/ygunknwy")) {

                        var myRequest = new LoginRequest() {
                            Username = UserName,
                            Password = ((PasswordBox)parameter).Password
                        };

                        var response = bus.Request<LoginRequest, LoginResponse>(myRequest);

                    if (response.Status == OperationStatus.Successfull)
                    {
                        var sampleMessageDialog = new SampleMessageDialog
                        {
                            Message = {Text = "Uspesno ste se ulogovali!"}
                        };

                        await DialogHost.Show(sampleMessageDialog);

                        ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new MainPage();
                    }
                    else
                    {
                        var sampleMessageDialog = new SampleMessageDialog {
                            Message = { Text = "Greska prilikom logovanja!" }
                        };

                        await DialogHost.Show(sampleMessageDialog);
                    }

                }
            }
            catch (Exception ex) {
                var sampleMessageDialog = new SampleMessageDialog {
                    Message = { Text = "Problem: " + ex.Message }
                };

                await DialogHost.Show(sampleMessageDialog);
            }


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
