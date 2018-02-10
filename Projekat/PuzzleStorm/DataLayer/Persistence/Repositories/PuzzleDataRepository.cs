using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;
using StormCommonData.Enums;

namespace DataLayer.Persistence.Repositories
{
    public class PuzzleDataRepository : Repository<PuzzleData>, IPuzzleDataRepository
    {
        public PuzzleDataRepository(StormContext context) : base(context)
        {

        }

        public StormContext StormContext => Context as StormContext;

        public PuzzleData GetPuzzle(int numberOfPieces)
        {
            return FirstOrDefault(x => x.NumberOfPieces == numberOfPieces);
        }
    }
}
