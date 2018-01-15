using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOLibrary.Enums;

namespace DTOLibrary.Broadcasts
{
    public class RoomsStateUpdate : BroadcastMessage
    {
        public RoomUpdateType UpdateType { get; set; }

        public int CreatorId { get; set; }
        public int RoomId { get; set; }
        public PuzzleDifficulty Level { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public bool IsPublic { get; set; }
    }
}
