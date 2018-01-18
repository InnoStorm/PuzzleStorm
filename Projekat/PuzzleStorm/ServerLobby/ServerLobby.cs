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
    class ServerLobby : StormServer
    {
        #region Worker Pools

        private BlockingCollection<LobbyWorker> _lobbyWorkerPool;

        #endregion

        #region StartupProcess

        protected override void StartupInit()
        {
            InitWorkerPool();
            BindWorkerMethods();
        }

        private void InitWorkerPool()
        {
            _lobbyWorkerPool = new BlockingCollection<LobbyWorker>();
            for (int i = 0; i < Config.DefaultWorkerPoolSize; i++)
                _lobbyWorkerPool.Add(new LobbyWorker() { Id = i, Communicator = Communicator });
        }

        private void BindWorkerMethods()
        {
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

            Communicator.RespondAsync<DeleteRoomRequest, DeleteRoomResponse>(request =>
                 Task.Factory.StartNew(() =>
                 {
                     var worker = _lobbyWorkerPool.Take();
                     try
                     {
                         return worker.DeleteRoom(request);
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
        }

        #endregion

        #region ShutdownProcess

        protected override void ShutdownCleanUp()
        {
            _lobbyWorkerPool.Dispose();
        }

        #endregion

        static void Main(string[] args)
        {
            ServerInstance = new ServerLobby();
            ServerInstance.Start();
        }


    }
}
