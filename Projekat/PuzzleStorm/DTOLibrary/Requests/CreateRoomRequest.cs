using DTOLibrary.Enums;

namespace DTOLibrary.Requests
{
    public class CreateRoomRequest : PostLoginRequest
    {
        public PuzzleDifficulty Level { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public string Password { get; set; }
    }
}
