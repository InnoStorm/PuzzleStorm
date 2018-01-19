using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using MaterialDesignThemes.Wpf;

namespace Client
{
    public class LobbyPageViewModel : BaseViewModel
    {
        #region Properties

        public List<LobbyJoinedPlayerViewModel> JoinedPlayersItems { get; set; }

        public RoomsPropsViewModel RoomStatus { get; set; }

        #endregion

        #region Commands

        public ICommand StartReadyCommand { get; set; }

        public ICommand BackCommand { get; set; }

        #endregion

        #region Constuctors

        public LobbyPageViewModel()
        {
            StartReadyCommand = new RelayCommand(StartReadyGame);

            if (Player.Instance.Creator)
                BackCommand = new RelayCommand(async () => await DeleteRoomAsync());
            else
                BackCommand = new RelayCommand(async () => await DisconnectFromRoom());

            JoinedPlayersItems = new List<LobbyJoinedPlayerViewModel>()
            {
                new LobbyJoinedPlayerViewModel()
                {
                    Username = "Pera1",
                    Ready = true
                },
                new LobbyJoinedPlayerViewModel()
                {
                    Username = "Pera2",
                    Ready = false
                },
                new LobbyJoinedPlayerViewModel()
                {
                    Username = "Pera3",
                    Ready = false
                }
            };
        }

        #endregion

        #region Metods


        //Create button f-ja
        public void StartReadyGame()
        {

        }

        #region DeleteRoom

        private DeleteRoomResponse DeleteRoomTask(DeleteRoomRequest request)
        {
            try
            {
                return RabbitBus.Instance.Bus.Request<DeleteRoomRequest, DeleteRoomResponse>(request);
            }
            catch (Exception ex)
            {
                return new DeleteRoomResponse()
                {
                    Status = OperationStatus.Exception,
                    Details = ex.Message
                };
            }
        }

        private async Task DeleteRoomAsync()
        {

            DeleteRoomRequest request = new DeleteRoomRequest()
            {
                RequesterId = Player.Instance.Id,
                RoomId = Player.Instance.RoomId
            };

            Task<DeleteRoomResponse> task = new Task<DeleteRoomResponse>(() => DeleteRoomTask(request));
            task.Start();

            //UI
            var popup = new LoadingPopup()
            {
                Message = {Text = "Deleting a room.."}
            };

            DeleteRoomResponse response = null;

            await DialogHost.Show(popup, async delegate(object sender, DialogOpenedEventArgs args)
            {
                response = await task;
                args.Session.Close(false);
            });

            if (response.Status != OperationStatus.Exception)
            {
                if (response.Status == OperationStatus.Successfull)
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = {Text = "Room deleted Successfull!"}
                    };

                    await DialogHost.Show(sampleMessageDialog);

                    ((MainWindow) Application.Current.MainWindow).MainFrame.Content = new MainPage();
                }
                else
                {
                    var sampleMessageDialog = new SampleMessageDialog
                    {
                        Message = {Text = "Delete room error!\n" + response.Details}
                    };

                    await DialogHost.Show(sampleMessageDialog);
                }
            }
            else //DOSO EXCEPTION
            {
                var sampleMessageDialog = new SampleMessageDialog
                {
                    Message = {Text = "Exception: " + response.Details}
                };

                await DialogHost.Show(sampleMessageDialog);
            }
        }
        #endregion

        #region Disconnect

        private Task DisconnectFromRoom() {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}
