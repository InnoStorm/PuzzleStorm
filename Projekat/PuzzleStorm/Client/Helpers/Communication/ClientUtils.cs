﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Communicator;
using DTOLibrary.Broadcasts;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;
using MaterialDesignThemes.Wpf;
using StormCommonData;
using StormCommonData.Enums;
using StormCommonData.Events;

namespace Client.Helpers.Communication
{
    public static class ClientUtils
    {
        #region Requests

        public static async Task<TResponse> PerformRequestAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> RequestFunc, TRequest request, string loadingMessage = "Please wait...")
            where TRequest : Request, new()
            where TResponse : Response, new()
        {
            var popup = new LoadingPopup()
            {
                Message = { Text = loadingMessage }
            };

            TResponse response = null;

            await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args)
            {
                response = await RequestFunc(request);
                args.Session.Close(false);
            });


            if (response?.Status == OperationStatus.Successfull)
                return response;


            await DialogHost.Show(new SampleMessageDialog
                {
                    Message = { Text = "Failed: " + response.Details }
                }
            );

            return null;
        }

        #endregion

        #region Subscribe

        private static string SubscriptionId
        {
            get
            {
                if (Player.Instance.Id <= 0)
                    throw new Exception("ClientUtils: SubscriptionId is invalid! PlayerId iz not initialized");

                return Player.Instance.Id.ToString();
            }
        }

        #endregion

        #region EventsForwarder

        public static event EventHandler<StormEventArgs<RoomsStateUpdate>> RoomChanged
        {
            add
            {
                API.Instance.RoomChanged -= value;
                API.Instance.RoomChanged += value;
            }
            remove => API.Instance.RoomChanged -= value;
        }

        public static event EventHandler<StormEventArgs<RoomPlayerUpdate>> InRoomChange
        {
            add
            {
                API.Instance.InRoomChange -= value;
                API.Instance.InRoomChange += value;
            }
            remove => API.Instance.InRoomChange -= value;
        }

        #endregion

        #region States

        public static class SwitchState
        {
            private static ISubscriptionResult _roomChangesSubscription;
            private static ISubscriptionResult _inRoomChangesSubscription;

            //A
            public static void LoginToHome()
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.All());
            }

            //B
            public static void HomeToCreateRoom()
            {
                Unsubscribe(_roomChangesSubscription);
            }

            //C
            public static void CreateRoomToLobbyOwner(int createdRoomId)
            {
                ResubscribeInRoom(RouteGenerator.RoomUpdates.InRoom.All(createdRoomId));
            }

            //D
            public static void LobbyOwnerToGameplay()
            {
                Unsubscribe(_roomChangesSubscription);
                Unsubscribe(_inRoomChangesSubscription);
            }

            //E
            public static void LobbyOwnerToHome()
            {
                Unsubscribe(_inRoomChangesSubscription);
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.All());
            }

            //F
            public static void HomeToLobbyJoiner(int joinedRoomId)
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.All(joinedRoomId));
                ResubscribeInRoom(RouteGenerator.RoomUpdates.InRoom.All(joinedRoomId));
            }

            //G
            public static void LobbyJoinerToHome()
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.All());
                Unsubscribe(_inRoomChangesSubscription);
            }

            //H I
            public static void LobbyJoinerToGameplay()
            {
                Unsubscribe(_roomChangesSubscription);
                Unsubscribe(_inRoomChangesSubscription);
            }

            //J
            public static void CreateRoomToHome()
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.All());
            }

            private static void ResubscribeRoomChanges(string routingKey)
            {
                API.Instance.Unsubscribe(_roomChangesSubscription);
                _inRoomChangesSubscription = API.Instance.SubscribeInRoomChanges(SubscriptionId, routingKey);
            }

            private static void ResubscribeInRoom(string routingKey)
            {
                API.Instance.Unsubscribe(_roomChangesSubscription);
                _roomChangesSubscription = API.Instance.SubscribeRoomChanges(SubscriptionId, routingKey);
            }

            private static void Unsubscribe(ISubscriptionResult subscription)
            {
                API.Instance.Unsubscribe(subscription);
            }
            
        }
        
        #endregion

        public static void UpdateGUI(Action action)
        {
            Application.Current.Dispatcher.InvokeAsync(action);
        }
    }
}
