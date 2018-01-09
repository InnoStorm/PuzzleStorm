using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
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
