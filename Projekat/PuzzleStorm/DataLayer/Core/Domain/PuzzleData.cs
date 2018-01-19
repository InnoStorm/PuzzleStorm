using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    public class PuzzleData
    {
        public int Id { get; set; }
        public int NumberOfPieces { get; set; }
        public string PicturePath { get; set; }
        public virtual IList<PieceData> ListOfPieces { get; set; }
        public virtual IList<Game> GamesWithThisPuzzle { get; set; }

        public PuzzleData()
        {
            ListOfPieces = new List<PieceData>();
        }
    }
}
