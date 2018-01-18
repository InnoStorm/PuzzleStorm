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
                _lobbyWorkerPool.Add(new LobbyWorker() { Id = i });
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
