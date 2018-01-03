using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain
{
    public enum Difficulty
    {
        Begginer = 1,
        Intermediate = 2,
        Advanced = 3
    }

    public class Room
    {
        public int Id { get; set; }
        public Player Creator { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public Difficulty Difficulty { get; set; }
        public bool IsPublic { get; set; }
        public IList<int> RoomScoreBoard { get; set; }
    }
}
