using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Enums;

namespace DTOLibrary.SubDTOs
{
    public class RoomInfo
    {
        public int RoomId { get; set; }
        public int CreatorUsername { get; set; }
        public PuzzleDifficulty Level { get; set; }
        public int MaxPlayers { get; set; }
        public int NumberOfRounds { set; get; }
    }
}
