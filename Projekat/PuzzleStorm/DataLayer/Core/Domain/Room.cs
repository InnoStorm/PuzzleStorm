﻿using System.Collections.Generic;

namespace DataLayer.Core.Domain
{
    
    public class Room
    {
        public int Id { get; set; }
        public virtual RoomProperties Properties { get; set; }
        public IList<Player> ListOfPlayers { get; set; }
        public IList<Puzzle> ListOfPuzzles { get; set; }
        public bool IsPublic { get; set; }
        public string Password { get; set; }
        public virtual Game CurrentGame { get; set; }

        public Room()
        {
            ListOfPlayers = new List<Player>();
            ListOfPuzzles = new List<Puzzle>();
        }
    }
}