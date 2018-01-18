using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public virtual Puzzle PuzzleForGame { get; set; }
        public virtual Room RoomForThisGame { get; set; }
        public virtual IList<Player> ListOfPlayers { get; set; }


        public Game()
        {
            ListOfPlayers = new List<Player>();
        }
    }
}
