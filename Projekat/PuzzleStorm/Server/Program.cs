using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core.Domain;
using Server.Persistence;
using Server.Persistence.Repositories;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var unitOfWork = new UnitOfWork(new StormContext()))
            {
                /*
                var userr = unitOfWork.Users.Get(1);

                Console.Write("Unesite username: ");
                var userName = Console.ReadLine();
                Console.Write("Unesite password: ");
                var passWord = Console.ReadLine();


                var user = new User
                {
                    Username = userName,
                    Password = passWord
                };

                unitOfWork.Users.Add(user);

                unitOfWork.Complete();

                var player = new Player()
                {
                    UserForPlayer = unitOfWork.Users.FindByUsername(userName)
                };

                unitOfWork.Players.Add(player);

                unitOfWork.Complete();
                */

                RoomProperties roomprops = new RoomProperties()
                {
                    Level = Difficulty.Advanced,
                    MaxPlayers = 4,
                    NumberOfRounds = 3
                };

                unitOfWork.RoomProperties.Add(roomprops);
                unitOfWork.Complete();

                //Console.WriteLine("Uspesno ste dodali igraca");

                //User a = unitOfWork.Users.FindByUsername("marijaaa");


                //Console.WriteLine(user.Username);
            }
        }
    }
}
