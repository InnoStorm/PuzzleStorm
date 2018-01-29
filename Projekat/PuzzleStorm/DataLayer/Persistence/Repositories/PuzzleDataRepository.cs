using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class PuzzleDataRepository : Repository<PuzzleData>, IPuzzleDataRepository
    {
        public PuzzleDataRepository(StormContext context) : base(context)
        {

        }

        public StormContext StormContext => Context as StormContext;
    }
}
