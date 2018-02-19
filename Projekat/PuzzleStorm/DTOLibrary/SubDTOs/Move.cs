using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOLibrary.SubDTOs
{
    public class Move
    {
        public int PositionFrom { get; set; }
        public int PositionTo { get; set; }
        public Player PlayedBy { get; set; }
        public bool IsSuccessfull { get; set; }
    }
}
