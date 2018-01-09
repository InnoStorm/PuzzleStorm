using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
{
    public class RoomPropertiesConfiguration : EntityTypeConfiguration<RoomProperties>
    {
        public RoomPropertiesConfiguration()
        {
            HasRequired(r => r.RoomOfThisProperties)
                .WithRequiredPrincipal(r => r.Properties);
        }
    }
}
