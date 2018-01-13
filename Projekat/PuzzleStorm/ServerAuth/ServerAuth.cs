using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;
using ServerAuth.Workers;
using DTOLibrary.Requests;
using DTOLibrary.Responses;

namespace ServerAuth
{
    class ServerAuth
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
                        Shutdown();
                    return false;
                default:
                    return false;
            }
        }
        #endregion

        private static BlockingCollection<AuthWorker> _workersPool;
        private static IBus _communicatorBus;
        
        static void Main(string[] args)
        {
            Startup();
            MainLoop();
            Shutdown();
        }
        
        private static void Startup()
        {
            Log("Starting server...");

            InitWorkerPool();
            CreateConnection();
            BindWorkerMethods();
        }

        private static void CreateConnection()
        {
            _communicatorBus = RabbitHutch.CreateBus(Config.ConnectionString);
        }

        private static void MainLoop()
        {
            Log("Auth server is running...");
            Log("Press CTRL + C or type 'exit' to shutdown " + Environment.NewLine);

            while (true)
            {
                string input = Console.ReadLine();
                if (input?.ToLower() == "exit")
                    break;
            }
        }

        private static void Shutdown()
        {
            Log("Shuting down server...");
          
            _communicatorBus?.Dispose();
            _workersPool?.Dispose();

            Log("Server is down");
            Console.ReadKey();
        }
        
        public static void Log(string message)
        {
            Console.WriteLine($"[{DateTime.Now}] {message}");
        }

        private static void InitWorkerPool()
        {
            _workersPool = new BlockingCollection<AuthWorker>();
            for (int i = 0; i < Config.DefaultWorkerPoolSize; i++)
                _workersPool.Add(new AuthWorker() { Id = i });
        }

        private static void BindWorkerMethods()
        {
            _communicatorBus.RespondAsync<RegistrationRequest, RegistrationResponse>(request =>
                Task.Factory.StartNew(() => {
                    var worker = _workersPool.Take();
                    try
                    {
                        return worker.Register(request);
                    }
                    finally
                    {
                        _workersPool.Add(worker);
                    }
                }));
            
            _communicatorBus.RespondAsync<LoginRequest, LoginResponse>(request =>
                Task.Factory.StartNew(() => {
                    var worker = _workersPool.Take();
                    try
                    {
                        return worker.Login(request);
                    }
                    finally
                    {
                        _workersPool.Add(worker);
                    }
                }));

        }
    }
}
