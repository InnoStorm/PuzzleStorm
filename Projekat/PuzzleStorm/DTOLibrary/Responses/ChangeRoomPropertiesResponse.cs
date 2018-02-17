using StormCommonData.Enums;

namespace DTOLibrary.Responses
{
    public class ChangeRoomPropertiesResponse : Response
    {
        public PuzzleDifficulty Difficulty { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
    }
}
