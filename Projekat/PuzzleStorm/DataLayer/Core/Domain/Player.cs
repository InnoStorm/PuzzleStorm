using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsLogged { get; set; }
        public string AuthToken { get; set; }
        public int Score { get; set; }
        public bool IsReady { get; set; }
        public virtual Room CurrentRoom { get; set; }
        public virtual IList<Room> OwnedRooms { get; set; }

        public Player()
        {
            OwnedRooms = new List<Room>();
        }
    }
}
