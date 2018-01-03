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
        public IList<Player> ListOfPlayers { get; set; }
        public Puzzle Puzzle { get; set; }
        public IList<int> GameScoreBoard { get; set; }
        public Player CurrentPointer { get; set; }
    }
}
