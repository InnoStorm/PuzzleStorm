using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Domain
{
    public class Puzzle
    {
        public int Id { get; set; }
        public int NumberOfPieces { get; set; }
        public string PicturePath { get; set; }
        public IList<Piece> ListOfPieces { get; set; }
    }
}
