using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Piece
    {
        public int Id { get; set; }
        public int SeqNumber { get; set; }
        public string PartPath { get; set; }
        public bool State { get; set; } //0 - Not placed; 1 - Placed 
        public Puzzle ParentPuzzle { get; set; }
    }
}
