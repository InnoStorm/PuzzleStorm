using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Server.Domain;


namespace Server.EntityTypeConfigurations
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
