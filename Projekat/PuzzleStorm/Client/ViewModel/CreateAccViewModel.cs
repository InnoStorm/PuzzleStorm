﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;
using EasyNetQ;

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
            CreateAccCommand = new RelayCommandWithParameter(async (parameter) => { await Create(parameter); });
            BackCommand = new RelayCommand(BackToLoginPage);
        }

        #endregion

        #region Metods

        private RegistrationResponse CreateTask(RegistrationRequest request) {
            try {
                return RabbitBus.Instance.Bus.Request<RegistrationRequest, RegistrationResponse>(request);
            }
            catch (Exception ex) {
                return new RegistrationResponse() {
                    Status = OperationStatus.Exception,
                    Details = ex.Message
                };
            }
        }

        // Login f-ja
        public async Task Create(object parameter)
        {
 
            RegistrationRequest myRequest = new RegistrationRequest()
            {
                Username = UserName,
                Password = ((PasswordBox) parameter).Password,
                Email = Email
            };

            Task<RegistrationResponse> task = new Task<RegistrationResponse>(() => CreateTask(myRequest));
            task.Start();

            //UI LOADING 
            var popup = new LoadingPopup() {
                Message = { Text = "Creating new account.." }
            };

            RegistrationResponse response = null;

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
                        Message = {Text = "Uspesno ste se kreirali novi nalog!"}
                    };

                    await DialogHost.Show(sampleMessageDialog);

                    ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new LoginPage();
                }
                else
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = {Text = "Greska prilikom kreiranja novog naloga.\n" + response.Details}
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

        public void BackToLoginPage()
        {

            //((WindowViewModel)((MainWindow)Application.Current.MainWindow).DataContext).CurrentPage = ApplicationPage.Login;
            ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new LoginPage();
            //((MainWindow) Application.Current.MainWindow).MainFrame.Source = new System.Uri("Pages/LoginPage.xaml", UriKind.Relative);

        }
        #endregion
    }
}
