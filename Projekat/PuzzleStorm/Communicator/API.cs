using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Broadcasts;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;
using EasyNetQ.Loggers;
using RabbitMQ.Client.MessagePatterns;
using StormCommonData;
using StormCommonData.Events;

namespace Communicator
{
    public sealed class API : IDisposable
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["RabbitMQConnection"].ConnectionString;

        #region Singleton

        private static readonly Lazy<API> APILazyInstance = new Lazy<API>(() => new API());

        public static API Instance => APILazyInstance.Value;
        
        private readonly IBus _bus;

        private API()
        {
            var logger = new ConsoleLogger();
            _bus = RabbitHutch.CreateBus(ConnectionString, x => x.Register<IEasyNetQLogger>(_ => logger));
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
                return await _bus.RequestAsync<TRequest, TResponse>(request);
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

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            return await RequestAsync<LoginRequest, LoginResponse>(request);
        }

        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request)
        {
            return await RequestAsync<RegistrationRequest, RegistrationResponse>(request);
        }

        /// <summary>
        /// Sign out
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SignOutResponse> SignOutAsync(SignOutRequest request)
        {
            return await RequestAsync<SignOutRequest, SignOutResponse>(request);
        }
        
        //Room
        public async Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request)
        {
            return await RequestAsync<CreateRoomRequest, CreateRoomResponse>(request);
        }

        public async Task<CancelRoomResponse> CancelRoomAsync(CancelRoomRequest request)
        {
            return await RequestAsync<CancelRoomRequest, CancelRoomResponse>(request);
        }

        public async Task<GetAllRoomsResponse> GetAllRoomsAsync(GetAllRoomsRequest request)
        {
            return await RequestAsync<GetAllRoomsRequest, GetAllRoomsResponse>(request);
        }

        public async Task<RoomCurrentStateResponse> GetRoomCurrentStateAsync(RoomCurrentStateRequest request)
        {
            return await RequestAsync <RoomCurrentStateRequest, RoomCurrentStateResponse>(request);
        }

        public async Task<ChangeRoomPropertiesResponse> ChangeRoomPropertiesAsync(ChangeRoomPropertiesRequest request)
        {
            return await RequestAsync <ChangeRoomPropertiesRequest, ChangeRoomPropertiesResponse> (request);
        }


        //InRoom
        public async Task<ChangeStatusResponse> ChangeStatusAsync(ChangeStatusRequest request)
        {
            return await RequestAsync <ChangeStatusRequest, ChangeStatusResponse>(request);
        }

        public async Task<JoinRoomResponse> JoinRoomAsync(JoinRoomRequest request)
        {
            return await RequestAsync <JoinRoomRequest, JoinRoomResponse>(request);
        }

        public async Task<LeaveRoomResponse> LeaveRoomAsync(LeaveRoomRequest request)
        {
            return await RequestAsync <LeaveRoomRequest, LeaveRoomResponse>(request);
        }

        public async Task<StartRoomResponse> StartRoomAsync(StartRoomRequest request)
        {
            return await RequestAsync <StartRoomRequest, StartRoomResponse>(request);
        }


        //Puzzle
        public async Task<AddPuzzlesResponse> AddPuzzleAsync(AddPuzzlesRequest request)
        {
            return await RequestAsync<AddPuzzlesRequest, AddPuzzlesResponse>(request);
        }

        #endregion

        #region Subscribe

        public ISubscriptionResult SubscribeRoomChanges(
            EventHandler<PuzzleStormEventArgs<RoomsStateUpdate>> RoomChangesHandler, 
            string subscriptionId,
            string routingKey = "#"
        )
        {
            RoomChanged -= RoomChangesHandler;
            RoomChanged += RoomChangesHandler;
            
            return _bus.SubscribeAsync<RoomsStateUpdate>(
                subscriptionId,
                message => Task.Factory.StartNew(() =>
                {
                    OnRoomChange(message);
                }),
                x => x.WithTopic(routingKey));
        }

        #endregion

        #region Unsubscribe

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
            _bus?.Dispose();
        }
        #endregion

    }
}
