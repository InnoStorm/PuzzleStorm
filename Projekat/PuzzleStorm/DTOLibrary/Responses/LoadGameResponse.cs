using DTOLibrary.SubDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.Responses
{
    public class LoadGameResponse : Response
    {
        public int GameId { get; set; }
        public List<String> PiecesPaths { get; set; }
        public int CurrentPlayerId { get; set; }
        public List<Player> ListOfPlayers { get; set; }
        public Scoreboard ScoreBoard { get; set; }
        
        public LoadGameResponse()
        {
            ListOfPlayers = new List<Player>();
        }
    }
}
