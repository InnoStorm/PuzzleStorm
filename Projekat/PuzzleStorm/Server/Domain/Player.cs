using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int Score { get; set; }
    }
}
