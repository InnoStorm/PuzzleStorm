using Server.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Persistence.EntityTypeConfigurations
{
    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            Property(u => u.Username)
                .IsRequired();

            Property(u => u.Password)
                .IsRequired();

            HasOptional(u => u.PlayerForUser)
                .WithRequired(p => p.UserForPlayer);
        }
    }
}
