using System;
using System.Collections.Generic;
using Communicator;
using DTOLibrary.Broadcasts;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using EasyNetQ;
using StormCommonData.Enums;
using StormCommonData.Events;

namespace DEBUGConsole
{
    class Program
    {
        private const string ConnectionString =
            "host=sheep.rmq.cloudamqp.com;" +
            "virtualHost=ygunknwy;" +
            "username=ygunknwy;" +
            "password=pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ;" +
            "timeout=0";

        private static IBus rabbit;

        //static void Main(string[] args)
        //{
        //    try
        //    {
        //        rabbit = RabbitHutch.CreateBus(ConnectionString);

        //        bool loop = true;

        //        while (loop)
        //        {
        //            Console.Clear();
        //            Console.WriteLine("Choose option:");
        //            Console.WriteLine(
        //                $"1. Invalid Registration()" + Environment.NewLine +
        //                $"2. Valid Registration()" + Environment.NewLine +
        //                $"3. Invalid Login()" + Environment.NewLine +
        //                $"4. Valid Login()" + Environment.NewLine +
        //                $"5. Invalid Signout()" + Environment.NewLine +
        //                $"6. Valid Signout()" + Environment.NewLine +
        //                $"7. Test Function()" + Environment.NewLine +
        //                $"8. Add puzzles to database()" + Environment.NewLine +
        //                $"10. Start room()" + Environment.NewLine +

        //                Environment.NewLine +
        //                $"0. Exit");

        //            string input = Console.ReadLine();
        //            Console.WriteLine("Executing...");

        //            switch (input)
        //            {
        //                case "0":
        //                    loop = false;
        //                    break;
        //                case "1":
        //                    RegistrationInvalid();
        //                    break;
        //                case "2":
        //                    RegistrationValid();
        //                    break;
        //                case "3":
        //                    LoginInvalid();
        //                    break;
        //                case "4":
        //                    LoginValid();
        //                    break;
        //                case "5":
        //                    SignOutInvalid();
        //                    break;
        //                case "6":
        //                    SignOutValid();
        //                    break;
        //                case "7":
        //                    TestFunction();
        //                    break;
        //                case "8":
        //                    AddPuzzlesToDatabase();
        //                    break;
        //                case "9":
        //                    ChangeStatusForPlayer();
        //                    break;
        //                case "10":
        //                    StartRoom();
        //                    break;

        //                default:
        //                    break;
        //            }
        //        }               
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Greska: {ex.Message}");
        //        Console.WriteLine($"Vise: {ex.InnerException?.Message}");
        //    }
        //    finally
        //    {
        //        rabbit.Dispose();
        //    }
        //}

        static void Main(string[] args)
        {
            using (var api = Communicator.API.Instance)
            {
                api.RequesterId = 3;
                var subscription = api.SubscribeRoomChanges(RoomChangesHandler, "Room.#");
                
                Console.WriteLine("Subscribed...");
                Console.ReadKey();

                api.Unsubscribe(subscription);

                Console.WriteLine("Unsubscribed..");
                Console.ReadKey();
            }
        }

        private static void RoomChangesHandler(object sender, PuzzleStormEventArgs<RoomsStateUpdate> e)
        {
            Console.WriteLine("New Update: " + e.Data.UpdateType);
        }

        private static void SignOutValid()
        {
            object response;

            response = rabbit.Request<SignOutRequest, SignOutResponse>(new SignOutRequest()
            {
                RequesterId = 1
            });
        }

        private static void SignOutInvalid()
        {
            object response;

            response = rabbit.Request<SignOutRequest, SignOutResponse>(new SignOutRequest()
            {
                RequesterId = -1
            });
        }

        private static void LoginValid()
        {
            object response;

            response = rabbit.Request<LoginRequest, LoginResponse>(new LoginRequest()
            {
                Username = "dacha204",
                Password = "666"
            });
        }

        private static void LoginInvalid()
        {
            object response;

            response = rabbit.Request<LoginRequest, LoginResponse>(new LoginRequest()
            {
                Username = "",
                Password = "666"
            });

            response = rabbit.Request<LoginRequest, LoginResponse>(new LoginRequest()
            {
                Username = "dacha204",
                Password = ""
            });

            response = rabbit.Request<LoginRequest, LoginResponse>(new LoginRequest()
            {
                Username = "dacha204",
                Password = "666112"
            });
        }

        private static void RegistrationValid()
        {
            object
            response = rabbit.Request<RegistrationRequest, RegistrationResponse>(new RegistrationRequest()
            {
                Username = "dacha204",
                Email = "dacha@mail.com",
                Password = "666"
            });
        }

        private static void RegistrationInvalid()
        {
            object response;

            response = rabbit.Request<RegistrationRequest, RegistrationResponse>(new RegistrationRequest()
            {
                Username = "testUser1",
                Email = "no-reply@mail.ru",
                Password = "666"
            });

            response = rabbit.Request<RegistrationRequest, RegistrationResponse>(new RegistrationRequest()
            {
                Username = "testUser1",
                Email = "no-reply@mail.ru",
                Password = "666"
            });

            response = rabbit.Request<RegistrationRequest, RegistrationResponse>(new RegistrationRequest()
            {
                Username = "testUser2",
                Email = "no-reply@mail.ru",
                Password = ""
            });

            response = rabbit.Request<RegistrationRequest, RegistrationResponse>(new RegistrationRequest()
            {
                Username = "",
                Email = "no-reply@mail.ru",
                Password = "666"
            });

            response = rabbit.Request<RegistrationRequest, RegistrationResponse>(new RegistrationRequest()
            {
                Username = "user name",
                Email = "no-reply@mail.ru",
                Password = "666"
            });

        }

        private static void TestFunction()
        {
            object response;
            Console.WriteLine("Creating room...");
            response = rabbit.Request<CreateRoomRequest, CreateRoomResponse>(new CreateRoomRequest()
            {
                RequesterId = 2,
                MaxPlayers = 10,
                Password = "666",
                Difficulty = PuzzleDifficulty.Easy,
                NumberOfRounds = 10
            });

            int soba = ((CreateRoomResponse) response).RoomId;

            Console.WriteLine("Joining room...");
            response = rabbit.Request<JoinRoomRequest, JoinRoomResponse>(new JoinRoomRequest()
            {
                RoomId = soba,
                Password = "666",
                RequesterId = 1,
            });

            Console.WriteLine("Listing room...");
            response = rabbit.Request<GetAllRoomsRequest, GetAllRoomsResponse>(new GetAllRoomsRequest()
            {
               RequesterId = 2
            });

            Console.WriteLine("Changing room prop...");
            response = rabbit.Request<ChangeRoomPropertiesRequest, ChangeRoomPropertiesResponse>(new ChangeRoomPropertiesRequest()
            {
                RequesterId = 2,
                RoomId = soba,
                MaxPlayers = 22,
                Difficulty = PuzzleDifficulty.Hard,
                NumberOfRounds = 204,
            });

            Console.WriteLine("Leaving room...");
            response = rabbit.Request<CancelRoomRequest, CancelRoomResponse>(new CancelRoomRequest()
            {
                RequesterId = 2,
                RoomId = soba
            });

        }

        private static void AddPuzzlesToDatabase()
        {
            object response = rabbit.Request<AddPuzzlesRequest, AddPuzzlesResponse>(new AddPuzzlesRequest());
        }

        private static void ChangeStatusForPlayer()
        {
            object response = rabbit.Request<ChangeStatusRequest, ChangeStatusResponse>(new ChangeStatusRequest()
            {
                RequesterId = 2,
                IAmReady = false
            });
        }

        private static void StartRoom()
        {
            object response = rabbit.Request<StartRoomRequest, StartRoomResponse>(new StartRoomRequest()
            {
                RequesterId = 2,
                RoomId = 7
            });
        }
    }
}
