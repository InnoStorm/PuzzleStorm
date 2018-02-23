using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StormCommonData.Enums;

namespace DTOLibrary.SubDTOs
{
    public class RoomInfo
    {
        public int RoomId { get; set; }
        public string CreatorUsername { get; set; }
        public PuzzleDifficulty Difficulty { get; set; }
        public int MaxPlayers { get; set; }
        public int NumberOfRounds { set; get; }
        public bool IsPublic { get; set; }
    }
}
