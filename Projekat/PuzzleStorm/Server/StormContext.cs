using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Server.Domain;

namespace Server
{
    class StormContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<Puzzle> Puzzles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public StormContext()
            : base("DefaultConnection")
        {

        }
    }
}
