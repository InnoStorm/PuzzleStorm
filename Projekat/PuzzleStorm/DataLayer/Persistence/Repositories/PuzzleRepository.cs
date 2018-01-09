using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class PuzzleRepository : Repository<Puzzle>, IPuzzleRepository
    {
        public PuzzleRepository(StormContext context) : base(context)
        {

        }





        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
