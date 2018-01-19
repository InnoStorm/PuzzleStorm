using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    public class Game
    {
        public int Id { get; set; }
        public virtual PuzzleData PuzzleForGame { get; set; }
        public virtual Room RoomForThisGame { get; set; }
    }
}
