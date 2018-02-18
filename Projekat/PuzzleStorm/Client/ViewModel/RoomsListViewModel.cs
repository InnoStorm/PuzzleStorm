using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Broadcasts;
using System.Threading.Tasks;
using System.Windows.Threading;
using StormCommonData.Enums;
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

        #endregion
    }
}
