using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    public class Room
    {
        public int Id { get; set; }
        public int NumberOfRounds { get; set; }
        public int MaxPlayers { get; set; }
        public int Difficulty { get; set; }
        public virtual IList<Player> ListOfPlayers { get; set; }
        public virtual Player Owner { get; set; }
        public bool IsPublic { get; set; }
        public string Password { get; set; }
        public virtual Game CurrentGame { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsStarted { get; set; }

        public Room()
        {
            ListOfPlayers = new List<Player>();
        }
    }
}
