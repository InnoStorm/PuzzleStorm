using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
{
    public class PlayerConfiguration : EntityTypeConfiguration<Player>
    {
        public PlayerConfiguration()
        {
            HasMany(p => p.OwnedRooms)
                .WithRequired(r => r.Owner);
        }
    }
}
