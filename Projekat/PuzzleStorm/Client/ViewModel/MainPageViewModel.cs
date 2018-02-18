﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Client.Helpers.Communication;
using Client.Helpers.Enums;
using Communicator;
using DTOLibrary.Broadcasts;
using StormCommonData.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using MaterialDesignThemes.Wpf;
using StormCommonData;
using StormCommonData.Events;

namespace Client {

    /// <summary>
    /// ViewModel za main stranicu
    /// </summary>
    public class MainPageViewModel : BaseViewModel {

        #region Private

        #endregion

        #region Properties

        public ObservableCollection<RoomsPropsViewModel> RoomsItemsList { get; set; }

        public bool NoRoomLabel { get; set; } = true;

        #endregion

        #region Commands

        /// <summary>
        /// Komanda koja vodi do sve sobe klikom na tri tacke (ALL ROOMS BUTTON TOO)
        /// </summary>
        public ICommand TriTackeCommand { get; set; }

        /// <summary>
        /// Komadna za log out iz main stranice
        /// </summary>
        public ICommand LogOutCommand { get; set; }

        /// <summary>
        /// Komanda koja otvara prozor za kreiranje nove sobe
        /// </summary>
        public ICommand CreateNewRoomCommand { get; set; }

        #endregion

        #region Constructors

        public MainPageViewModel()
        {
            TriTackeCommand = new RelayCommand(TriTackeButton);
            LogOutCommand = new RelayCommand(async () => await LogOutButtonAsync());

            CreateNewRoomCommand = new RelayCommand(CreateNewRoom);

            RoomsItemsList = new ObservableCollection<RoomsPropsViewModel>();

            if (ListRooms.Instance.RoomsItemsList.Count == 0)
                Application.Current.Dispatcher.InvokeAsync(InitRooms);
                //InitializingAsync();
            else
                RoomsItemsList = ListRooms.Instance.RoomsItemsList;

            NoRoomLabel = false;

            //TODO DETECT transition
            //Demo
            ActivateTransition(WindowTransition.LoginToHome);
        }


        private void ActivateTransition(WindowTransition transition)
        {
            //Samo prelazi ToHome (To-this-view)
            switch (transition)
            {
                case WindowTransition.LoginToHome:
                    ClientUtils.SwitchState.LoginToHome();
                    break;
                case WindowTransition.LobbyOwnerToHome:
                    ClientUtils.SwitchState.LobbyOwnerToHome();
                    break;
                case WindowTransition.LobbyJoinerToHome:
                    ClientUtils.SwitchState.LobbyJoinerToHome();
                    break;
                case WindowTransition.CreateRoomToHome:
                    ClientUtils.SwitchState.CreateRoomToHome();
                    break;
                case WindowTransition.HomeToHome:
                    break;
                default:
                    return;
            }

            ClientUtils.RoomChanged += OnRoomChange;
            //ClientUtils.InRoomChange += OnInRoomChange;
        }

        #region InitRooms

        private async Task InitRooms()
        {
            GetAllRoomsRequest myRequest = new GetAllRoomsRequest() {
                   RequesterId = Player.Instance.Id
            };

            var response = await ClientUtils.PerformRequestAsync(API.Instance.GetAllRoomsAsync, myRequest);
            if (response == null) return;

            foreach (RoomInfo room in response.List)
            {
                ListRooms.Instance.RoomsItemsList.Add(
                    new RoomsPropsViewModel()
                    {
                        RoomId = room.RoomId,
                        Name = "Room #" + room.RoomId,
                        By = room.CreatorUsername,
                        Difficulty = CastDifficulty(room.Difficulty),
                        MaxPlayers = room.MaxPlayers.ToString(),
                        Rounds = room.NumberOfRounds.ToString(),
                        Visibility = Visibility.Visible,
                        Locked = !room.IsPublic
                    }
                );
            }

            RoomsItemsList = ListRooms.Instance.RoomsItemsList;
            NoRoomLabel = false;
        }

        private string CastDifficulty(PuzzleDifficulty level) {
            switch (level)
            {
                case PuzzleDifficulty.Easy:
                    return 16.ToString();
                case PuzzleDifficulty.Medium:
                    return 25.ToString();
                case PuzzleDifficulty.Hard:
                    return 36.ToString();
                default:
                    return "X";
            }
        }

        #endregion

        #endregion

        #region Metode

        //private void OnInRoomChange(object o, StormEventArgs<RoomPlayerUpdate> stormEventArgs)
        //{
        //    throw new NotImplementedException();
        //}

        private void OnRoomChange(object o, StormEventArgs<RoomsStateUpdate> stormEventArgs)
        {
            ClientUtils.UpdateGUI(() =>
            {
                var update = stormEventArgs.Data;

                switch (update.UpdateType)
                {
                    case RoomUpdateType.Created:
                    case RoomUpdateType.BecameAvailable:
                        RoomsItemsList.Add(new RoomsPropsViewModel()
                            {
                                By = update.Creator.Username,
                                RoomId = update.RoomId,
                                MaxPlayers = update.MaxPlayers.ToString(),
                                Difficulty = CastDifficulty(update.Level),
                                Locked = !update.IsPublic,
                                Name = update.Creator.Username,
                                Rounds = update.NumberOfRounds.ToString(),
                            });
                        ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                        break;

                    case RoomUpdateType.Modified:
                        var room = RoomsItemsList.Single(x => x.RoomId == update.RoomId);
                        room.Difficulty = CastDifficulty(update.Level);
                        room.MaxPlayers = update.MaxPlayers.ToString();
                        room.Rounds = update.NumberOfRounds.ToString();
                        break;

                    case RoomUpdateType.Deleted:
                    case RoomUpdateType.Started:
                    case RoomUpdateType.Filled:
                        RoomsItemsList.Remove(RoomsItemsList.SingleOrDefault(x => x.RoomId == update.RoomId));
                        ListRooms.Instance.RoomsItemsList = RoomsItemsList;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        #region TriTacke

        /// <summary>
        /// F-ja kad klikne dugme tri tacke ili na all rooms dugme
        /// </summary>
        public void TriTackeButton() {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new RoomsListPage();
        }

        #endregion

        #region LogOut
        /// <summary>
        /// Dugme za logout
        /// </summary>
        public async Task LogOutButtonAsync() {

            SignOutRequest request = new SignOutRequest() {
                RequesterId = Player.Instance.Id
            };


            var response = await ClientUtils.PerformRequestAsync(API.Instance.SignOutAsync,request);
            if (response == null) return;

            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new LoginPage();
        }

        #endregion

        #region CreateNewRoom

        /// <summary>
        /// F-ja za novu sobu
        /// </summary>
        public void CreateNewRoom() {
            ((MainWindow)Application.Current.MainWindow).MainFrame.Content = new CreateRoomPage(); ;
        }

        #endregion

        #endregion

    }
}
