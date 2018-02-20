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
using StormCommonData.Enums;

namespace ServerGame
{
    public class ServerGame : StormServer<ServerGame>
    {
        #region Singleton

        private ServerGame() { }

        #endregion

        #region Worker Pools

        private List<GameWorker> ActiveWorkerPool { get; set; }
        private readonly object _lockpad = new object();
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
            lock (_lockpad)
            {
                ActiveWorkerPool = new List<GameWorker>();
            }
        }

        private void BindWorkerMethods()
        {
            Log("Binding workers...");

            Communicator.RespondAsync<StartGameRequest, StartGameResponse>(request =>
                Task.Factory.StartNew(() =>
                {
                    var worker = new GameWorker(Communicator, this);
                    var response = worker.StartGame(request);

                    if (response.Status == OperationStatus.Successfull)
                        HireWorker(worker);
                    
                    return response;
                }));
        }

        private void HireWorker(GameWorker worker)
        {
            lock (_lockpad)
            {
                ActiveWorkerPool.Add(worker);
            }
        }

        private void ReleaseWorker(GameWorker worker)
        {
            lock (_lockpad)
            {
                ActiveWorkerPool.Remove(worker);
            }
        }
        #endregion

        #region Disposable

        #endregion
    }
}