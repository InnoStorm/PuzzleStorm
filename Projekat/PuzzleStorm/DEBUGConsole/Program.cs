using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Requests;
using DTOLibrary.Responses;
using EasyNetQ;

namespace DEBUGConsole {
    class Program
    {
        private const string ConnectionString = "amqp://ygunknwy:pAncRrH8Gxk3ULDyy-Wju7NIqdBThwCJ@sheep.rmq.cloudamqp.com/ygunknwy";

        static void Main(string[] args) {


            // registracija
            /*
            try 
            {
                using (var bus = RabbitHutch.CreateBus(ConnectionString)) 
                {
                    //Console.WriteLine("Press any key to start DEMO");

                    Random rnd = new Random(DateTime.Now.Millisecond);

                    
                        var myRequest = new RegistrationRequest() {
                            Email = "demo@mail.com",
                            Username = $"User666",
                            Password = "pass"
                        };

                        var response = bus.Request<RegistrationRequest, RegistrationResponse>(myRequest);
                        Console.WriteLine($"Username:{response.Username} Status: {response.Status}");
                    
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            */


            // room current state request
            try
            {
                using (var bus = RabbitHutch.CreateBus(ConnectionString))
                {
                        var myRequest = new RoomCurrentStateRequest()
                        {
                            //RequesterId = 1,
                            RoomId = 2
                        };

                        var response = bus.Request<RoomCurrentStateRequest, RoomCurrentStateResponse>(myRequest);

                        Console.WriteLine(response.NumberOfRounds);
                        Console.WriteLine($"Status: {response.Status}");
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
