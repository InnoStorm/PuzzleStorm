using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using StormCommonData.Enums;
using Client.Helpers.Communication;
using Communicator;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client {

    public class RoomsPropsViewModel {

        #region Constructors

        public RoomsPropsViewModel() {

            //ChangeCommand = new RelayCommand(ChangeButton);

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

        public ICommand ChangeCommand { get; set; }

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

            JoinRoomResponse response = await ClientUtils.PerformRequestAsync(API.Instance.JoinRoomAsync, request,
                "Joining..");

            if (response == null) return;

            Player.Instance.RoomId = this.RoomId;

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LobbyPage();
        }

        #endregion

        #endregion
    }
}
