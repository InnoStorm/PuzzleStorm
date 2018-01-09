using System.Data.Entity;
using DataLayer.Core.Domain;
using DataLayer.Persistence.EntityTypeConfigurations;

namespace DataLayer.Persistence
{
    public class StormContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Piece> Pieces { get; set; }
        public DbSet<Puzzle> Puzzles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomProperties> RoomProperties { get; set; }

        public StormContext()
            : base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GameConfiguration());
            modelBuilder.Configurations.Add(new PieceConfiguration());
            modelBuilder.Configurations.Add(new PlayerConfiguration());
            modelBuilder.Configurations.Add(new PuzzleConfiguration());
            modelBuilder.Configurations.Add(new RoomConfiguration());
            modelBuilder.Configurations.Add(new RoomPropertiesConfiguration());
            modelBuilder.Configurations.Add(new UserConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
