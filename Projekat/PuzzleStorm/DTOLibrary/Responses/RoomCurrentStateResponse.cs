using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.SubDTOs;
using StormCommonData.Enums;

namespace DTOLibrary.Responses
{
    public class RoomCurrentStateResponse : Response
    {
        public PuzzleDifficulty Difficulty { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public List<Player> Players { get; set; }
        public Player Creator { get; set; }

        public RoomCurrentStateResponse()
        {
            Players = new List<Player>();
        }
    }
}
