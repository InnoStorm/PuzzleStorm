using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DTOLibrary.Broadcasts;
using System.Threading.Tasks;
using System.Windows.Threading;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using StormCommonData.Enums;
using EasyNetQ;
using StormCommonData.Events;

namespace Client {

    public class RoomsListViewModel : BaseViewModel {

        #region Private

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
            ActivateTransition(WindowTransition.AllRoomsEnter);
            
            RoomsItemsList = ListRooms.Instance.RoomsItemsList;

            BackCommand = new RelayCommand(BackToMainPage);

        }

        #endregion

        #region Metods

        public void BackToMainPage()
        {
            ActivateTransition(WindowTransition.AllRoomsExit);
            
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new MainPage();
        }

        private void ActivateTransition(WindowTransition transition)
        {
            switch (transition)
            {
                case WindowTransition.AllRoomsEnter:
                    ClientUtils.SwitchState.AllRoomsEnter();
                    ClientUtils.RoomChanged += OnRoomChange;
                    break;
                case WindowTransition.AllRoomsExit:
                    ClientUtils.SwitchState.AllRoomsExit();
                    ClientUtils.RoomChanged -= OnRoomChange;
                    break;
            }
        }

        private void OnRoomChange(object o, StormEventArgs<RoomsStateUpdate> stormEventArgs) {
            ClientUtils.UpdateGUI(() => {
                var update = stormEventArgs.Data;

                switch (update.UpdateType) {
                    case RoomUpdateType.Created:
                    case RoomUpdateType.BecameAvailable:
                        RoomsItemsList.Add(new RoomsPropsViewModel() {
                            By = update.Creator.Username,
                            RoomId = update.RoomId,
                            MaxPlayers = update.MaxPlayers.ToString(),
                            Difficulty = update.Level.ToString(),
                            Locked = !update.IsPublic,
                            Name = "Room #" + update.RoomId,
                            Rounds = update.NumberOfRounds.ToString(),
                        });
                        ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                        break;

                    case RoomUpdateType.Modified:
                        var room = RoomsItemsList.FirstOrDefault(x => x.RoomId == update.RoomId);
                        int i = RoomsItemsList.IndexOf(room);
                        RoomsItemsList[i] = new RoomsPropsViewModel()
                        {
                            By = update.Creator.Username,
                            RoomId = update.RoomId,
                            MaxPlayers = update.MaxPlayers.ToString(),
                            Difficulty = update.Level.ToString(),
                            Locked = !update.IsPublic,
                            Name = "Room #" + update.RoomId,
                            Rounds = update.NumberOfRounds.ToString()
                        };
                        break;

                    case RoomUpdateType.Deleted:
                    case RoomUpdateType.Started:
                    case RoomUpdateType.Filled:
                        if (RoomsItemsList.Any(x => x.RoomId == update.RoomId))
                        {
                            RoomsItemsList.Remove(RoomsItemsList.Single(x => x.RoomId == update.RoomId));
                            ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        #endregion
    }
}
