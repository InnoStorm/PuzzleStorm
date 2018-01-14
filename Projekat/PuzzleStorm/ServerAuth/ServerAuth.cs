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
using ServerAuth.Workers;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using Server;

namespace ServerAuth
{
    public class ServerAuth : StormServer
    {
        #region Worker Pools

        private BlockingCollection<AuthWorker> _authWorkerPool;

        #endregion

        #region StartupProcess

        protected override void StartupInit()
        {
            InitWorkerPool();
            BindWorkerMethods();
        }

        private void InitWorkerPool()
        {
            _authWorkerPool = new BlockingCollection<AuthWorker>();
            for (int i = 0; i < Config.DefaultWorkerPoolSize; i++)
                _authWorkerPool.Add(new AuthWorker() { Id = i });
        }

        private void BindWorkerMethods()
        {
            Communicator.RespondAsync<RegistrationRequest, RegistrationResponse>(request =>
                Task.Factory.StartNew(() =>
                {
                    var worker = _authWorkerPool.Take();
                    try
                    {
                        return worker.Register(request);
                    }
                    finally
                    {
                        _authWorkerPool.Add(worker);
                    }
                }));

            Communicator.RespondAsync<LoginRequest, LoginResponse>(request =>
                Task.Factory.StartNew(() =>
                {
                    var worker = _authWorkerPool.Take();
                    try
                    {
                        return worker.Login(request);
                    }
                    finally
                    {
                        _authWorkerPool.Add(worker);
                    }
                }));
        }

        #endregion

        #region ShutdownProcess

        protected override void ShutdownCleanUp()
        {
            _authWorkerPool.Dispose();
        }

        #endregion

        static void Main(string[] args)
        {
            ServerInstance = new ServerAuth();
            ServerInstance.Start();
        }
    }
}
