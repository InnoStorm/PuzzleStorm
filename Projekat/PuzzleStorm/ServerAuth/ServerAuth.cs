using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class ServerAuth : StormServer<ServerAuth>
    {
        #region Singleton

        private ServerAuth()
        {
        }
        
        #endregion
        
        #region WorkerPools

        private BlockingCollection<AuthWorker> _authWorkerPool;

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
            Log("Initializing auth worker pool...");

            _authWorkerPool = new BlockingCollection<AuthWorker>();
            for (int i = 0; i < Config.DefaultWorkerPoolSize; i++)
                _authWorkerPool.Add(new AuthWorker(Communicator)
                {
                    Id = i,
                    NewWorkerLogMessage = OnNewWorkerLogMessage,  
                });
        }

        private void BindWorkerMethods()
        {
            Log("Binding workers...");

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

            Communicator.RespondAsync<SignOutRequest, SignOutResponse>(request =>
                Task.Factory.StartNew(() =>
                {
                    var worker = _authWorkerPool.Take();
                    try
                    {
                        return worker.SignOut(request);
                    }
                    finally
                    {
                        _authWorkerPool.Add(worker);
                    }
                }));
        }

        #endregion

        #region Disposable

        public override void Dispose()
        {
            _authWorkerPool?.Dispose();
            
            base.Dispose();
        }

        #endregion
    }
}
