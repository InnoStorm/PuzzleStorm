using System;
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

        public static async Task<TResponse> PerformRequestAsync<TRequest, TResponse>(Func<TRequest, Task<TResponse>> RequestFunc, TRequest request, string loadingMessage)
            where TRequest : Request, new()
            where TResponse : Response, new()
        {
            var popup = new LoadingPopup()
            {
                Message = { Text = loadingMessage }
            };

            TResponse response = null;


            if (!String.IsNullOrEmpty(loadingMessage))
            {

                await DialogHost.Show(popup, async delegate (object sender, DialogOpenedEventArgs args)
                {
                    response = await RequestFunc(request);
                    args.Session.Close(false);
                });

            } else
            {
                response = await RequestFunc(request);
            }

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

        public static event EventHandler<StormEventArgs<GameUpdate>> GameUpdated
        {
            add
            {
                API.Instance.GameUpdated -= value;
                API.Instance.GameUpdated += value;
            }
            remove => API.Instance.GameUpdated -= value;
        }

        public static event EventHandler<StormEventArgs<LoadGameResponse>> LoadGameResponded
        {
            add
            {
                API.Instance.LoadGameResponded -= value;
                API.Instance.LoadGameResponded += value;
            }
            remove => API.Instance.LoadGameResponded -= value;
        }
        
        #endregion

        #region States

        public static class SwitchState
        {
            private static ISubscriptionResult _roomChangesSubscription;
            private static ISubscriptionResult _inRoomChangesSubscription;
            private static ISubscriptionResult _gameUpdatesSubscription;
            private static IDisposable _loadGameResponseActivation;

            public static void GamePlayDemo()
            {
                ResubscribeGameUpdates(RouteGenerator.GameUpdates.GamePlay.Filter.All(Player.Instance.RoomId));
            }

            public static void ActivateLoadGameResponse(int playerId)
            {
                _loadGameResponseActivation = API.Instance.ActivateReceiveLoadGameResponse(playerId);
            }

            public static void DeactivateLoadGameResponse()
            {
                _loadGameResponseActivation.Dispose();
            }

            //PROBA
            public static void HomeEnter()
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All());
            }

            public static void HomeExit()
            {
                Unsubscribe(_roomChangesSubscription);
            }

            public static void AllRoomsEnter() {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All());
            }

            public static void AllRoomsExit() {
                Unsubscribe(_roomChangesSubscription);
            }

            public static void LobbyEnter(int joinedRoomId) {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All(joinedRoomId));
                ResubscribeInRoom(RouteGenerator.RoomUpdates.InRoom.Filter.All(joinedRoomId));
            }

            public static void LobbyExit() {
                Unsubscribe(_roomChangesSubscription);
                Unsubscribe(_inRoomChangesSubscription);
            }

            //A
            public static void LoginToHome()
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All());
            }

            //B
            public static void HomeToCreateRoom()
            {
                Unsubscribe(_roomChangesSubscription);
            }

            //C
            public static void CreateRoomToLobbyOwner(int createdRoomId)
            {
                ResubscribeInRoom(RouteGenerator.RoomUpdates.InRoom.Filter.All(createdRoomId));
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
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All());
            }

            //F
            public static void HomeToLobbyJoiner(int joinedRoomId)
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All(joinedRoomId));
                ResubscribeInRoom(RouteGenerator.RoomUpdates.InRoom.Filter.All(joinedRoomId));
            }

            //G
            public static void LobbyJoinerToHome()
            {
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All());
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
                ResubscribeRoomChanges(RouteGenerator.RoomUpdates.Room.Filter.All());
            }

            public static void HomeToLogin()
            {
                Unsubscribe(_inRoomChangesSubscription);
                Unsubscribe(_roomChangesSubscription);
            }


            //Helper funcs
            private static void ResubscribeRoomChanges(string routingKey)
            {
                API.Instance.Unsubscribe(_roomChangesSubscription);
                _roomChangesSubscription = API.Instance.SubscribeRoomChanges(SubscriptionId, routingKey);
            }

            private static void ResubscribeInRoom(string routingKey)
            {
                API.Instance.Unsubscribe(_inRoomChangesSubscription);
                _inRoomChangesSubscription = API.Instance.SubscribeInRoomChanges(SubscriptionId, routingKey);
            }

            private static void Unsubscribe(ISubscriptionResult subscription)
            {
                API.Instance.Unsubscribe(subscription);
            }

            public static void ResubscribeGameUpdates(string routingKey)
            {
                API.Instance.Unsubscribe(_gameUpdatesSubscription);
                _gameUpdatesSubscription = API.Instance.SubscribeGameUpdates(SubscriptionId, routingKey);
            }
            
        }
        
        #endregion

        public static void UpdateGUI(Action action)
        {
            DoAsync(action);
        }

        public static void DoAsync(Action action)
        {
            Application.Current.Dispatcher.InvokeAsync(action);
        }
    }
}
