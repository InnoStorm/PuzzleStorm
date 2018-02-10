using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
{
    public class GameConfiguration : EntityTypeConfiguration<Game>
    {
        public GameConfiguration()
        {
            HasRequired(g => g.RoomForThisGame)
                .WithOptional(r => r.CurrentGame);
        }
    }
}
