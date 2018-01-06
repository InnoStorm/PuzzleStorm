using Server.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Persistence.EntityTypeConfigurations
{
    public class PieceConfiguration : EntityTypeConfiguration<Piece>
    {
        public PieceConfiguration()
        {
            
        }
    }
}
