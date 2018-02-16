using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client {

    public class RoomsPropsViewModel {

        #region Constructors

        public RoomsPropsViewModel() {
            JoinCommand = new RelayCommandWithParameter(async (paramater) => await JoinInRoomAsync(paramater));
        }

        #endregion

        #region Properties

        public int RoomId { get; set; }

        public string Name { get; set; }

        public string By { get; set; }

        public string Difficulty { get; set; }

        public string MaxPlayers { get; set; }

        public string Rounds { get; set; }

        public bool Locked { get; set; }

        public Visibility Visibility { get; set; }

        #endregion

        #region Commands

        public ICommand JoinCommand { get; set; }

        #endregion

        #region Metods

        #region Joining

        public async Task JoinInRoomAsync(object parametar)
        {
            JoinRoomRequest request = new JoinRoomRequest()
            {
                RequesterId = Player.Instance.Id,
                RoomId = this.RoomId
            };

            if (Locked)
            {
                var dialog = new SimpleTextBoxDialog()
                {
                    Message = {Text = "ENTER PASSWORD"},
                    DialogButton = {Content = "JOIN"}
                };

                await DialogHost.Show(dialog, delegate(object sender, DialogClosingEventArgs args)
                {
                    if (args.Parameter != null && !string.IsNullOrWhiteSpace((string) args.Parameter))
                    {
                        request.Password = (string) args.Parameter;
                    }
                });
            }

            Task<JoinRoomResponse> task = new Task<JoinRoomResponse>(() => JoinRoomTask(request));
            task.Start();

            //UI LOADING 
            var popup = new LoadingPopup() {
                Message = { Text = "Joining.." }
            };

            JoinRoomResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args) {
                response = await task;
                args.Session.Close(false);
            });
            //

            if (response.Status != OperationStatus.Exception)
            {
                if (response.Status == OperationStatus.Successfull) {
                    var sampleMessageDialog = new SampleMessageDialog {
                        Message = { Text = "Joining Successfull!" }
                    };

                    await DialogHost.Show(sampleMessageDialog);

                    Player.Instance.RoomId = this.RoomId;

                    ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LobbyPage();
                }
                else {

                    var sampleMessageDialog = new SampleMessageDialog {
                        Message = { Text = "Joining error!\n" + response.Details }
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

        private JoinRoomResponse JoinRoomTask(JoinRoomRequest request) {

            try {
                return RabbitBus.Instance.Bus.Request<JoinRoomRequest, JoinRoomResponse>(request);
            }
            catch (Exception ex) {
                return new JoinRoomResponse() {
                    Status = OperationStatus.Exception,
                    Details = ex.Message
                };
            }
        }

        #endregion

        #endregion
    }
}
