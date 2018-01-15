using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Enums;
using DTOLibrary.SubDTOs;

namespace DTOLibrary.Responses
{
    public class RoomCurrentStateResponse : Response
    {
        public PuzzleDifficulty Level { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public List<Player> Players { get; set; }
    }
}
