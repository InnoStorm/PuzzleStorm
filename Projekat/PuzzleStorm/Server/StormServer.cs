using System;
using System.Runtime.InteropServices;
using System.Threading;
using EasyNetQ;

namespace Server {

    public abstract class StormServer
    {
        #region CleanExit

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        static EventHandler _handler;

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig)
        {
            switch (sig)
            {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                case CtrlType.CTRL_BREAK_EVENT:
                    ServerInstance.Shutdown();
                    return false;
                default:
                    return false;
            }
        }

        protected static StormServer ServerInstance { get; set; }

        protected StormServer()
        {
            ServerInstance = this;
        }
        #endregion
        
        protected IBus Communicator;

        public bool ServerRunning { get; protected set; }


        protected virtual void Startup()
        {
            Log("Starting server...");

            //CleanExit event handler
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            Log("Connecting to RabbitMQ system...");
            Communicator = RabbitHutch.CreateBus(Config.ConnectionString);
            Log("Connected");

            StartupInit();

            ServerRunning = true;
        }

        protected abstract void StartupInit();


        private void MainLoop()
        {
            Log("Server is running...");
            Log("Press CTRL + C to shutdown " + Environment.NewLine);

            while (ServerRunning)
            {
                Thread.Sleep(100);
            }

            Thread.Sleep(750);
        }


        private void Shutdown()
        {
            if (!ServerRunning) return;

            Log("Shuting down server...");

            ShutdownCleanUp();
            Communicator?.Dispose();

            ServerRunning = false;

            Log("Server is down");
        }

        protected abstract void ShutdownCleanUp();
       
        
        public void Start()
        {
            if (ServerRunning) return;

            Startup();
            MainLoop();
        }

        public void Stop()
        {
            if (!ServerRunning) return;

            Shutdown();
        }

        protected void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] {message}");
        }
    }
}
