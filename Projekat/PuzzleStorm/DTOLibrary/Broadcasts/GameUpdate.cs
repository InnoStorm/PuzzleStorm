using Player = DataLayer.Core.Domain.Player;
using DTOLibrary.SubDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Broadcasts
{
    public class GameUpdate
    {
        public Player CurrentPlayer { get; set; }
        public Move PlayedMove { get; set; }
        public Scoreboard Scoreboard { get; set; }
    }
}
