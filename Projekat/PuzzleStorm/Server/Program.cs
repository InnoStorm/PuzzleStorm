using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core.Domain;
using Server.Persistence;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var unitOfWork = new UnitOfWork(new StormContext()))
            {
                var user = unitOfWork.Users.Get(1);

                Console.WriteLine(user.Username);
            }
        }
    }
}
