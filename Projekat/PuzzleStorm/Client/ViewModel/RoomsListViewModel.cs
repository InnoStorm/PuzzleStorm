using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Broadcasts;
using System.Threading.Tasks;
using System.Windows.Threading;
using DTOLibrary.Enums;
using EasyNetQ;

namespace Client {

    public class RoomsListViewModel : BaseViewModel {

        #region Private

        private ISubscriptionResult subResult;

        #endregion

        #region Properties

        public ObservableCollection<RoomsPropsViewModel> RoomsItemsList { get; set; }

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
                                //var rem = RoomsItemsList.Where(x => x.RoomId == request.RoomId);
                                //RoomsItemsList.Remove(rem);

                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => {

                                    RoomsPropsViewModel rem = null;

                                    foreach (var v in RoomsItemsList)
                                    {
                                        if (v.RoomId == request.RoomId)
                                            rem = v;
                                    }

                                    if (rem != null)
                                        RoomsItemsList.Remove(rem);

                                    ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                                }));
                            }
                            else if (request.UpdateType == RoomUpdateType.Created)
                            {
                                Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                                {
                                    RoomsItemsList.Add(new RoomsPropsViewModel()
                                    {
                                        By = request.Creator.Username,
                                        RoomId = request.RoomId
                                    });

                                    ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                                }));
                            }
                        }),
                    x => x.WithTopic("#"));
        }

        #endregion
    }
}
