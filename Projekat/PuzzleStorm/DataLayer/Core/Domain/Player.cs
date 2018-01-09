namespace DataLayer.Core.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public virtual User UserForPlayer { get; set; }
        public int Score { get; set; }
        public bool IsReady { get; set; }
        public Game CurrentGame { get; set; }
        public Room CurrentRoom { get; set; }
    }
}
