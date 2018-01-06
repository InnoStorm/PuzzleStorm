using Server.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.EntityTypeConfigurations
{
    public class GameConfiguration : EntityTypeConfiguration<Game>
    {
        public GameConfiguration()
        {
            HasMany(g => g.ListOfPlayers)
                .WithOptional(p => p.CurrentGame);

            
        }
    }
}
