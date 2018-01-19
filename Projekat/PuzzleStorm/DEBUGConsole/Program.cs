using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Enums;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using DTOLibrary.SubDTOs;
using EasyNetQ;

namespace DEBUGConsole {
    class Program
    {
        private const string ConnectionString = "amqp://ygunknwy:pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ@sheep.rmq.cloudamqp.com/ygunknwy";

        static void Main(string[] args) {
            /*
            try 
            {
                using (var bus = RabbitHutch.CreateBus(ConnectionString)) 
                {
                    Console.WriteLine("Started");

                    List<RoomCurrentStateRequest> requests = new List<RoomCurrentStateRequest>();

                    requests.Add(new RoomCurrentStateRequest()
                    {
                        RequesterId = 3566,
                        RoomId = 11
                    });
                    
                    foreach (var request in requests)
                    {
                        var response = bus.Request<RoomCurrentStateRequest, RoomCurrentStateResponse>(request);

                        foreach (Player player in response.Players)
                        {
                            Console.WriteLine($"CreatedRoom => {player.Username} Status: {response.Status}");
                        }
                    }
                    
                    Console.WriteLine("END DEMO");
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            */
            try
            {
                using (var bus = RabbitHutch.CreateBus(ConnectionString))
                {
                    Console.WriteLine("Started");

                    List<DeleteRoomRequest> requests = new List<DeleteRoomRequest>();

                    requests.Add(new DeleteRoomRequest()
                    {
                        RoomId = 20
                    });

                    foreach (var request in requests)
                    {
                        var response = bus.Request<DeleteRoomRequest, DeleteRoomResponse>(request);
                        
                            Console.WriteLine($"DeletedRoom => {requests[0].RoomId} Status: {response.Status}");
                    }

                    Console.WriteLine("END DEMO");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }

        }
    }
}




//// room current state request
//try
//{
//    using (var bus = RabbitHutch.CreateBus(ConnectionString))
//    {
//        Console.WriteLine("Started");
//        Console.ReadLine();

//        var request = new GetAllRoomsRequest()
//        {
//            RequesterId = 0
//        };

//        var response = bus.Request<GetAllRoomsRequest, GetAllRoomsResponse>(request);

//        Console.WriteLine("Usernames:");
//        foreach (var roomInfo in response.List)
//        {
//            Console.WriteLine(roomInfo.CreatorUsername);
//        }

//        Console.WriteLine("End.");
//    }
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//    Console.ReadKey();
//}

//using (var bus = RabbitHutch.CreateBus(ConnectionString)) 
//{
//Console.WriteLine("Press any key to start DEMO");

//List<RegistrationRequest> requests = new List<RegistrationRequest>();

//requests.Add(new RegistrationRequest()
//{
//    Email = "example@mail.com",
//    Username = "marijaaa",
//    Password = "666"
//});

//requests.Add(new RegistrationRequest()
//{
//    Email = "example@mail.com",
//    Username = "savchaa",
//    Password = "666"
//});

//requests.Add(new RegistrationRequest()
//{
//    Email = "example@mail.com",
//    Username = $"stefan",
//    Password = "666"
//});

//requests.Add(new RegistrationRequest()
//{
//    Email = "example@mail.com",
//    Username = $"dacha204",
//    Password = "666"
//});

//requests.Add(new RegistrationRequest()
//{
//    Email = "example@mail.com",
//    Username = $"fifolino",
//    Password = "666"
//});

//foreach (var request in requests)
//{
//    var response = bus.Request<RegistrationRequest, RegistrationResponse>(request);

//    Console.WriteLine($"Registred => Username:{response.Username} Status: {response.Status}");
//}

//Console.WriteLine("END DEMO");
//}


//List<GetAllRoomsRequest> requests = new List<GetAllRoomsRequest>();

//requests.Add(new GetAllRoomsRequest()
//{
//RequesterId = 3566,
//});


//foreach (var request in requests)
//{
//var response = bus.Request<GetAllRoomsRequest, GetAllRoomsResponse>(request);

//Console.WriteLine($"CreatedRoom => {response.List[0].CreatorUsername} Status: {response.Status}");
//}
