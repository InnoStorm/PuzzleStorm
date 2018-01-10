using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
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
            Console.WriteLine("Starting up server...");
            InitServer();

            Console.WriteLine("Server is running...");
            Console.Read();

            Console.WriteLine("Server is shutting down...");
            _communicatorBus.Dispose();
        }
    }
}
