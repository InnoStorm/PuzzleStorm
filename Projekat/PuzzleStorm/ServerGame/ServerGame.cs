using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using ServerGame.Workers;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using Server;
using DTOLibrary.Broadcasts;

namespace ServerGame
{
    public class ServerGame : StormServer<ServerGame>
    {
        #region Singleton

        private ServerGame() { }

        #endregion

        #region Worker Pools

        private BlockingCollection<GameWorker> _gameWorkerPool;

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
            Log("Initializing game worker pool...");

            _gameWorkerPool = new BlockingCollection<GameWorker>();
            for (int i = 0; i < Config.DefaultWorkerPoolSize; i++)
                _gameWorkerPool.Add(new GameWorker(Communicator)
                {
                    Id = i,
                    NewWorkerLogMessage = OnNewWorkerLogMessage
                });
        }

        private void BindWorkerMethods()
        {
            Log("Binding workers...");

            Communicator.RespondAsync<MakeAMoveRequest, MakeAMoveResponse>(request =>
                 Task.Factory.StartNew(() =>
                 {
                     var worker = _gameWorkerPool.Take();
                     try
                     {
                         return worker.MakeAMove(request);
                     }
                     finally
                     {
                         _gameWorkerPool.Add(worker);
                     }
                 }));
        }
        #endregion

        #region Disposable

        public override void Dispose()
        {
            _gameWorkerPool?.Dispose();

            base.Dispose();
        }

        #endregion
    }
}