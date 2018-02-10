namespace DataLayer.Core.Domain
{
    public class PieceData
    {
        public int Id { get; set; }
        public int SeqNumber { get; set; }
        public string PartPath { get; set; }
        public virtual PuzzleData ParentPuzzle { get; set; }
    }
}
