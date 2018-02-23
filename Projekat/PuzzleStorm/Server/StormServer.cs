using System;
using System.Runtime.InteropServices;
using System.Threading;
using EasyNetQ;
using EasyNetQ.Loggers;
using StormCommonData;
using StormCommonData.Enums;
using StormCommonData.EventArgs;
using StormCommonData.Interfaces;

namespace Server
{

    public class StormServer<TServerImpl> : IDisposable, IStormServer
        where TServerImpl : StormServer<TServerImpl>
    {

        #region Singleton
        private static readonly Lazy<TServerImpl> _instance = new Lazy<TServerImpl>(CreateInstanceOfServer);

        private static TServerImpl CreateInstanceOfServer()
        {
            return Activator.CreateInstance(typeof(TServerImpl), true) as TServerImpl;
        }

        public static TServerImpl Instance => _instance.Value;

        protected StormServer(){}

        #endregion

        #region Properties

        protected IBus Communicator;

        public bool IsRunning { get; protected set; }

        #endregion

        #region Startup / Shutdown

        public void Start()
        {
            if (IsRunning)
            {
                Log("Trying to start server that is already started.", LogMessageType.Warning);
                return;
            }

            Log("Starting server...");
            
            StartupInit();

            IsRunning = true;

            Log("Server is running...");
        }

        protected virtual void StartupInit()
        {
            Log("Connecting to RabbitMQ system...");
            
            var logger = new ConsoleLogger();
            Communicator = RabbitHutch.CreateBus(Config.ConnectionString, 
                x => x.Register<IEasyNetQLogger>(_ => logger));

            Log("Connected");
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                Log("Trying to shutdown server that is already shutdown.", LogMessageType.Warning);
                return;
            }

            Log("Shutting down...");

            ShutdownCleanup();
            
            IsRunning = false;
            Log("Server is down");
        }

        protected virtual void ShutdownCleanup()
        {
            Dispose();
        }
        
        #endregion

        #region AbstractMethods

        #endregion

        #region Disposable

        public virtual void Dispose()
        {
            Communicator?.Dispose();
        }

        #endregion

        #region Events

        public event EventHandler<LogMessageArgs> NewLogMessage;

        protected virtual void OnNewLogMessage(LogMessageArgs msg)
            => NewLogMessage?.Invoke(this, msg);

        protected void OnNewWorkerLogMessage(object sender, LogMessageArgs msg)
            => OnNewLogMessage(msg);

        #endregion

        #region Utilities

        protected virtual void Log(string message, LogMessageType type = LogMessageType.Info)
        {
            OnNewLogMessage(new LogMessageArgs(
                message: $@"[{DateTime.Now}] {message}",
                type: type
            ));
        }

        #endregion
    }
}
