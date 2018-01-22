using System.Data.Entity;
using DataLayer.Core.Domain;
using DataLayer.Persistence.EntityTypeConfigurations;

namespace DataLayer.Persistence
{
    public class StormContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<PieceData> Pieces { get; set; }
        public DbSet<PuzzleData> Puzzles { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Room> Rooms { get; set; }

        public StormContext()
            : base("DefaultConnection")
        {
            Database.Log = DebugOutputString => System.Diagnostics.Debug.WriteLine(DebugOutputString);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new GameConfiguration());
            modelBuilder.Configurations.Add(new PieceDataConfiguration());
            modelBuilder.Configurations.Add(new PlayerConfiguration());
            modelBuilder.Configurations.Add(new PuzzleDataConfiguration());
            modelBuilder.Configurations.Add(new RoomConfiguration());
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
