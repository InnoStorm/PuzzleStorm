using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    public class Puzzle
    {
        public int Id { get; set; }
        public int NumberOfPieces { get; set; }
        public string PicturePath { get; set; }
        public virtual IList<Piece> ListOfPieces { get; set; }
        public virtual Room RoomOfPuzzle { get; set; }
        public virtual Game GameOfPuzzle { get; set; }

        public Puzzle()
        {
            ListOfPieces = new List<Piece>();
        }
    }
}
