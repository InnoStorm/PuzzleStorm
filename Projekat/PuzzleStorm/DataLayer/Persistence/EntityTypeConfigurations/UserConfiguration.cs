using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
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
