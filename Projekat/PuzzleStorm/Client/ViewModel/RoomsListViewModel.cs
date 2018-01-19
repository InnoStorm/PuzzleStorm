using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Broadcasts;
using System.Threading.Tasks;
using DTOLibrary.Enums;
using EasyNetQ;

namespace Client {

    public class RoomsListViewModel : BaseViewModel {

        #region Private

        private ISubscriptionResult subResult;

        #endregion

        #region Properties

        public List<RoomsPropsViewModel> RoomsItemsList { get; set; }

        #endregion

        #region Commands

        public ICommand BackCommand { get; set; }
        
        #endregion

        #region Constuctors

        public RoomsListViewModel()
        {

            RoomsItemsList = ListRooms.Instance.RoomsItemsList;

            BackCommand = new RelayCommand(BackToMainPage);

            Subscribe();

        }

        #endregion

        #region Metods

        public void BackToMainPage()
        {
            subResult.Dispose();

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        private void Subscribe()
        {
            subResult = RabbitBus.Instance.Bus.SubscribeAsync<RoomsStateUpdate>("cl_" + Player.Instance.Id, 
                    request =>
                        Task.Factory.StartNew(() => {
                            if (request.UpdateType == RoomUpdateType.Deleted)
                            {
                                RoomsItemsList.RemoveAll(x => x.RoomId == request.RoomId);
                                ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                            }
                        }),
                    x => x.WithTopic("#"));
        }

        #endregion
    }
}
