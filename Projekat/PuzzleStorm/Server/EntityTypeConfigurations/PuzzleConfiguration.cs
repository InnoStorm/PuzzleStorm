using Server.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.EntityTypeConfigurations
{
    public class PuzzleConfiguration : EntityTypeConfiguration<Puzzle>
    {
        public PuzzleConfiguration()
        {
            HasMany(p => p.ListOfPieces)
                .WithRequired(p => p.ParentPuzzle);

            HasRequired(p => p.GameOfPuzzle)
                .WithRequiredPrincipal(g => g.PuzzleForGame);
        }
    }
}
