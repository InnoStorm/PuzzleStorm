using System.Data.Entity.ModelConfiguration;
using DataLayer.Core.Domain;

namespace DataLayer.Persistence.EntityTypeConfigurations
{
    public class PuzzleDataConfiguration : EntityTypeConfiguration<PuzzleData>
    {
        public PuzzleDataConfiguration()
        {
            HasMany(p => p.GamesWithThisPuzzle)
                .WithRequired(g => g.PuzzleForGame);

            HasMany(p => p.ListOfPieces)
                .WithRequired(p => p.ParentPuzzle);
        }
    }
}
