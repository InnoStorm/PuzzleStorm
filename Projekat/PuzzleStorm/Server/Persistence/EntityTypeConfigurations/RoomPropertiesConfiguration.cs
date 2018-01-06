using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Server.Core.Domain;


namespace Server.Persistence.EntityTypeConfigurations
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
