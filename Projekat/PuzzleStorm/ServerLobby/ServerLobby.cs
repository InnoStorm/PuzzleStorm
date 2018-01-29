using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using ServerLobby.Workers;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using Server;
using DTOLibrary.Broadcasts;

namespace ServerLobby
{
    public class ServerLobby : StormServer<ServerLobby>
    {
        #region Singleton

        private ServerLobby() { }

        #endregion

        #region Worker Pools

        private BlockingCollection<LobbyWorker> _lobbyWorkerPool;

        #endregion

        #region StartupProcess

        protected override void StartupInit()
        {
            base.StartupInit();

            InitWorkerPool();
            BindWorkerMethods();
        }

        private void InitWorkerPool()
        {
            Log("Initializing lobby worker pool...");

            _lobbyWorkerPool = new BlockingCollection<LobbyWorker>();
            for (int i = 0; i < Config.DefaultWorkerPoolSize; i++)
                _lobbyWorkerPool.Add(new LobbyWorker(Communicator)
                {
                    Id = i,
                    NewWorkerLogMessage = OnNewWorkerLogMessage
                });
        }

        private void BindWorkerMethods()
        {
            Log("Binding workers...");

            Communicator.RespondAsync<RoomCurrentStateRequest, RoomCurrentStateResponse>(request =>
                 Task.Factory.StartNew(() =>
                 {
                     var worker = _lobbyWorkerPool.Take();
                     try
                     {
                         return worker.GiveInfoAboutRoom(request);
                     }
                     finally
                     {
                         _lobbyWorkerPool.Add(worker);
                     }
                 }));

            Communicator.RespondAsync<CreateRoomRequest, CreateRoomResponse>(request =>
                 Task.Factory.StartNew(() =>
                 {
                     var worker = _lobbyWorkerPool.Take();
                     try
                     {
                         return worker.CreateNewRoom(request);
                     }
                     finally
                     {
                         _lobbyWorkerPool.Add(worker);
                     }
                 }));

            Communicator.RespondAsync<CancelRoomRequest, CancelRoomResponse>(request =>
                 Task.Factory.StartNew(() =>
                 {
                     var worker = _lobbyWorkerPool.Take();
                     try
                     {
                         return worker.CancelRoom(request);
                     }
                     finally
                     {
                         _lobbyWorkerPool.Add(worker);
                     }
                 }));

            Communicator.RespondAsync<JoinRoomRequest, JoinRoomResponse>(request =>
                 Task.Factory.StartNew(() =>
                 {
                     var worker = _lobbyWorkerPool.Take();
                     try
                     {
                         return worker.JoinRoom(request);
                     }
                     finally
                     {
                         _lobbyWorkerPool.Add(worker);
                     }
                 }));

            Communicator.RespondAsync<ChangeRoomPropertiesRequest, ChangeRoomPropertiesResponse>(request =>
                Task.Factory.StartNew(() =>
                {
                    var worker = _lobbyWorkerPool.Take();
                    try
                    {
                        return worker.ChangeRoomProperties(request);
                    }
                    finally
                    {
                        _lobbyWorkerPool.Add(worker);
                    }
                }));

            Communicator.RespondAsync<GetAllRoomsRequest, GetAllRoomsResponse>(request =>
                Task.Factory.StartNew(() =>
                {
                    var worker = _lobbyWorkerPool.Take();
                    try
                    {
                        return worker.GetAllRooms(request);
                    }
                    finally
                    {
                        _lobbyWorkerPool.Add(worker);
                    }
                }));
                
        }

        #endregion

        #region Disposable

        public override void Dispose()
        {
            _lobbyWorkerPool?.Dispose();

            base.Dispose();
        }

        #endregion
    }
}
