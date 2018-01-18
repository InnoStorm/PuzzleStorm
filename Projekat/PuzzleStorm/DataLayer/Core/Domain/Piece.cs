namespace DataLayer.Core.Domain
{
    public class Piece
    {
        public int Id { get; set; }
        public int SeqNumber { get; set; }
        public string PartPath { get; set; }
        public bool State { get; set; } //0 - Not placed; 1 - Placed 
        public virtual Puzzle ParentPuzzle { get; set; }
    }
}
