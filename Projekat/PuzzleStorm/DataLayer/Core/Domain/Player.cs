namespace DataLayer.Core.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public virtual User UserForPlayer { get; set; }
        public int Score { get; set; }
        public bool IsReady { get; set; }
        public virtual Game CurrentGame { get; set; }
        public virtual Room CurrentRoom { get; set; }
    }
}
