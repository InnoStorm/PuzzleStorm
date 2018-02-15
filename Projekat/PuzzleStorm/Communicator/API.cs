using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Broadcasts;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;
using RabbitMQ.Client.MessagePatterns;
using StormCommonData;
using StormCommonData.Events;

namespace Communicator
{
    public sealed class API : IDisposable
    {
        #region Singleton

        private const string ConnectionString =
            "host=sheep.rmq.cloudamqp.com;" +
            "virtualHost=ygunknwy;" +
            "username=ygunknwy;" +
            "password=pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ;" +
            "timeout=0";

        private static readonly Lazy<API> APILazyInstance = new Lazy<API>(() => new API(ConnectionString));

        public static API Instance => APILazyInstance.Value;
        
        private readonly IBus Bus;

        private int _requesterId;

        public int RequesterId
        {
            get
            {
                if (_requesterId == 0)
                    throw new Exception("PuzzleStorm API: _requesterId not initialized!");

                return _requesterId;
            }
            set
            {
                if (value <= 0)
                    throw new Exception("Invalid value. Must be >0");

                _requesterId = value;
            }
        }

        private string SubscriptionId => RequesterId.ToString();

        private API(string connectionString)
        {
            Bus = RabbitHutch.CreateBus(connectionString);
        }

        #endregion

        #region API Functions

        #region Requests

        //Generic wrapper
        private async Task<TResponse> RequestAsync<TRequest, TResponse>(TRequest request)
            where TRequest : Request, new()
            where TResponse : Response, new()
        {
            try
            {
                return await Bus.RequestAsync<TRequest, TResponse>(request);
            }
            catch (Exception ex)
            {
                return new TResponse()
                {
                    Status = OperationStatus.Failed,
                    Details = "Timeout!" + Environment.NewLine + StormUtils.FlattenException(ex)
                };
            }
        }

        //Login
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            return await RequestAsync<LoginRequest, LoginResponse>(request);
        }

        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
            return await RequestAsync<RegistrationRequest, RegistrationResponse>(request);
        }

        
        public async Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request)
        {
            return await RequestAsync<CreateRoomRequest, CreateRoomResponse>(request);
        }

        #endregion

        #region Subscribing

        public ISubscriptionResult SubscribeRoomChanges(
            EventHandler<PuzzleStormEventArgs<RoomsStateUpdate>> RoomChangesHandler, 
            string routingKey = "#"
            )
        {
            RoomChanged -= RoomChangesHandler;
            RoomChanged += RoomChangesHandler;
            
            return Bus.SubscribeAsync<RoomsStateUpdate>(
                SubscriptionId,
                message => Task.Factory.StartNew(() =>
                    {
                        OnRoomChange(message);
                    }),
                x => x.WithTopic(routingKey));
        }

        #endregion

        #region Unsubsribing

        public void Unsubscribe(ISubscriptionResult subscription)
        {
            subscription.Dispose();
        }

        #endregion

        #region Events

        public event EventHandler<PuzzleStormEventArgs<RoomsStateUpdate>> RoomChanged;
        private void OnRoomChange(RoomsStateUpdate updateMessage)
        {
            RoomChanged?.Invoke(this, new PuzzleStormEventArgs<RoomsStateUpdate>(updateMessage));
        }


        

        #endregion

        #endregion

        #region Disposable

        public void Dispose()
        {
            Bus?.Dispose();
        }

        #endregion

    }
}
