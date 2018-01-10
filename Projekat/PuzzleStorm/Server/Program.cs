using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataLayer.Core.Domain;
using DataLayer.Persistence;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using Server.Workers;
using EasyNetQ;

namespace Server {
    class Program
    {
        #region CleanExit
        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);
        static EventHandler _handler;

        enum CtrlType {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType sig) {
            switch (sig) {
                case CtrlType.CTRL_C_EVENT:
                case CtrlType.CTRL_LOGOFF_EVENT:
                case CtrlType.CTRL_SHUTDOWN_EVENT:
                case CtrlType.CTRL_CLOSE_EVENT:
                    _communicatorBus.Dispose();
                    return false;
                default:
                    return false;
            }
        }

        
        #endregion


        private static BlockingCollection<UserAuthWorker> _workersPool;
        private static IBus _communicatorBus;

        static void InitServer()
        {
            _workersPool = new BlockingCollection<UserAuthWorker>();
            for (int i = 0; i < 10; i++) {
                _workersPool.Add(new UserAuthWorker() { Id = i });
            }

            _communicatorBus = RabbitHutch.CreateBus("amqp://ygunknwy:pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ@sheep.rmq.cloudamqp.com/ygunknwy");

            _communicatorBus.RespondAsync<RegistrationRequest, RegistrationResponse>(request =>
                Task.Factory.StartNew(() => {
                    var worker = _workersPool.Take();
                    try {
                        return worker.Register(request);
                    }
                    finally {
                        _workersPool.Add(worker);
                    }
                }));

            _communicatorBus.RespondAsync<LoginRequest, LoginResponse>(request =>
                Task.Factory.StartNew(() => {
                    var worker = _workersPool.Take();
                    try {
                        return worker.Login(request);
                    }
                    finally {
                        _workersPool.Add(worker);
                    }
                }));

        }

        static void Main(string[] args) 
        {
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);


            Console.WriteLine("Starting up server...");
            InitServer();

            Console.WriteLine("Server is running...");
            Console.Read();

            Console.WriteLine("Server is shutting down...");
            _communicatorBus.Dispose();
        }
    }
}
