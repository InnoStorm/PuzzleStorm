﻿using System;
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

        //Generic wrappers
        private TResponse Request<TRequest, TResponse>(TRequest request)
            where TRequest : Request, new()
            where TResponse : Response, new()
        {
            try
            {
                return _bus.Request<TRequest, TResponse>(request);
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
        public LoginResponse Login(LoginRequest request)
            => Request<LoginRequest, LoginResponse>(request);
        
        /// <summary>
        /// Login
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<LoginResponse> LoginAsync(LoginRequest request) 
            => await RequestAsync<LoginRequest, LoginResponse>(request);


        
        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public RegistrationResponse Register(RegistrationRequest request)
            => Request<RegistrationRequest, RegistrationResponse>(request);
        
        /// <summary>
        /// Create new account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<RegistrationResponse> RegisterAsync(RegistrationRequest request) 
            => await RequestAsync<RegistrationRequest, RegistrationResponse>(request);


        
        /// <summary>
        /// SignOut
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public Task<SignOutResponse> SignOut(SignOutRequest request)
            => RequestAsync<SignOutRequest, SignOutResponse>(request);

        /// <summary>
        /// SignOutAsync
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<SignOutResponse> SignOutAsync(SignOutRequest request) 
            => await RequestAsync<SignOutRequest, SignOutResponse>(request);





        //Room
        public CreateRoomResponse CreateRoom(CreateRoomRequest request)
            => Request<CreateRoomRequest, CreateRoomResponse>(request);

        public async Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request)
            => await RequestAsync<CreateRoomRequest, CreateRoomResponse>(request);



        public CancelRoomResponse CancelRoom(CancelRoomRequest request)
            => Request<CancelRoomRequest, CancelRoomResponse>(request);

        public async Task<CancelRoomResponse> CancelRoomAsync(CancelRoomRequest request)
            => await RequestAsync<CancelRoomRequest, CancelRoomResponse>(request);



        public GetAllRoomsResponse GetAllRooms(GetAllRoomsRequest request)
            => Request<GetAllRoomsRequest, GetAllRoomsResponse>(request);

        public async Task<GetAllRoomsResponse> GetAllRoomsAsync(GetAllRoomsRequest request)
            => await RequestAsync<GetAllRoomsRequest, GetAllRoomsResponse>(request);



        public RoomCurrentStateResponse GetRoomCurrentState(RoomCurrentStateRequest request)
            => Request<RoomCurrentStateRequest, RoomCurrentStateResponse>(request);

        public async Task<RoomCurrentStateResponse> GetRoomCurrentStateAsync(RoomCurrentStateRequest request)
            => await RequestAsync<RoomCurrentStateRequest, RoomCurrentStateResponse>(request);



        public ChangeRoomPropertiesResponse ChangeRoomProperties(ChangeRoomPropertiesRequest request)
            => Request<ChangeRoomPropertiesRequest, ChangeRoomPropertiesResponse>(request);

        public async Task<ChangeRoomPropertiesResponse> ChangeRoomPropertiesAsync(ChangeRoomPropertiesRequest request)
            => await RequestAsync<ChangeRoomPropertiesRequest, ChangeRoomPropertiesResponse>(request);



        //InRoom
        public ChangeStatusResponse ChangeStatus(ChangeStatusRequest request)
            => Request<ChangeStatusRequest, ChangeStatusResponse>(request);

        public async Task<ChangeStatusResponse> ChangeStatusAsync(ChangeStatusRequest request)
            => await RequestAsync<ChangeStatusRequest, ChangeStatusResponse>(request);



        public JoinRoomResponse JoinRoom(JoinRoomRequest request)
            => Request<JoinRoomRequest, JoinRoomResponse>(request);

        public async Task<JoinRoomResponse> JoinRoomAsync(JoinRoomRequest request)
            => await RequestAsync<JoinRoomRequest, JoinRoomResponse>(request);



        public LeaveRoomResponse LeaveRoom(LeaveRoomRequest request)
            => Request<LeaveRoomRequest, LeaveRoomResponse>(request);

        public async Task<LeaveRoomResponse> LeaveRoomAsync(LeaveRoomRequest request)
            => await RequestAsync<LeaveRoomRequest, LeaveRoomResponse>(request);



        public StartRoomResponse StartRoom(StartRoomRequest request)
            => Request<StartRoomRequest, StartRoomResponse>(request);

        public async Task<StartRoomResponse> StartRoomAsync(StartRoomRequest request)
            => await RequestAsync<StartRoomRequest, StartRoomResponse>(request);



        //Puzzle
        public AddPuzzlesResponse AddPuzzle(AddPuzzlesRequest request)
            => Request<AddPuzzlesRequest, AddPuzzlesResponse>(request);

        public async Task<AddPuzzlesResponse> AddPuzzleAsync(AddPuzzlesRequest request)
            => await RequestAsync<AddPuzzlesRequest, AddPuzzlesResponse>(request);

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
