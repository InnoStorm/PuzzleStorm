using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public virtual Puzzle PuzzleForGame { get; set; }
        public virtual Room RoomForThisGame { get; set; }
        public IList<Player> ListOfPlayers { get; set; }


        public Game()
        {
            ListOfPlayers = new List<Player>();
        }
    }
}
