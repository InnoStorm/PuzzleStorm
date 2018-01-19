using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
{
    public class PlayerConfiguration : EntityTypeConfiguration<Player>
    {
        public PlayerConfiguration()
        {
            HasOptional(p => p.CurrentRoom)
                .WithMany(r => r.ListOfPlayers);

            HasOptional(p => p.CurrentRoom)
                .WithRequired(r => r.Owner);
        }
    }
}
