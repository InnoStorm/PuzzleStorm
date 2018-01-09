using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
{
    public class RoomConfiguration : EntityTypeConfiguration<Room>
    {
        public RoomConfiguration()
        {
            HasMany(r => r.ListOfPlayers)
                .WithOptional(p => p.CurrentRoom);

            HasMany(r => r.ListOfPuzzles)
                .WithOptional(p => p.RoomOfPuzzle);

            HasOptional(r => r.CurrentGame)
                .WithRequired(g => g.RoomForThisGame);
        }
    }
}
