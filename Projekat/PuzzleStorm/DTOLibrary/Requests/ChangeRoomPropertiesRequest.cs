using DTOLibrary.Enums;

namespace DTOLibrary.Requests
{
    public class ChangeRoomPropertiesRequest : PostLoginRequest
    {
        public PuzzleDifficulty Level { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
    }
}
