namespace DataLayer.Core.Domain
{
    public enum Difficulty
    {
        Begginer = 1,
        Intermediate = 2,
        Advanced = 3
    }


    public class RoomProperties
    {
        public int Id { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public Difficulty Level { get; set; }
        public virtual Room RoomOfThisProperties { get; set; }
    }
}
