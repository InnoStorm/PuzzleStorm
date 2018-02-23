using StormCommonData.Enums;

namespace DTOLibrary.Requests
{
    public class ChangeRoomPropertiesRequest : PostLoginRequest
    {
        public PuzzleDifficulty Difficulty { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public int RoomId { get; set; }
    }
}
