using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Core.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public virtual User UserForPlayer { get; set; }
        public int Score { get; set; }
        public bool IsReady { get; set; }
        public Game CurrentGame { get; set; }
        public Room CurrentRoom { get; set; }
    }
}
