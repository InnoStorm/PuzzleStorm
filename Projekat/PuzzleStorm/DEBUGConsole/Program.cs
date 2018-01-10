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
            try 
            {
                using (var bus = RabbitHutch.CreateBus(ConnectionString)) 
                {
                    Console.WriteLine("Press any key to start DEMO");

                    Random rnd = new Random(DateTime.Now.Millisecond);

                    for (int i = 0; i < 50; i++) 
                    {
                        var myRequest = new RegistrationRequest() {
                            Email = "demo@mail.com",
                            Username = $"User{rnd.Next(50000)}",
                            Password = "pass"
                        };

                        var response = bus.Request<RegistrationRequest, RegistrationResponse>(myRequest);
                        Console.WriteLine($"Username:{response.Username} Status: {response.Status}");
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
        }
    }
}
