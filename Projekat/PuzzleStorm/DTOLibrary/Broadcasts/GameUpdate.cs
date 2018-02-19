using DTOLibrary.SubDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCommonData.Enums;

namespace DTOLibrary.Broadcasts
{
    public class GameUpdate
    {
        public GamePlayUpdateType UpdateType { get; set; }

        public Player CurrentPlayer { get; set; }
        public Move PlayedMove { get; set; }
        public Scoreboard Scoreboard { get; set; }
    }
}
