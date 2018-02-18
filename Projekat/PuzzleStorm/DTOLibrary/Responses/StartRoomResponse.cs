using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.SubDTOs;

namespace DTOLibrary.Responses
{
    public class StartRoomResponse : Response
    {
        public int GameId;
        public int PuzzleId;
        public List<String> PiecesPaths;
        public int CurrentPlayerId;
        public List<Player> ListOfPlayers;
    }
}
