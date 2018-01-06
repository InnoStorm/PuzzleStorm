using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration;
using Server.Domain;

namespace Server.EntityTypeConfigurations
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
