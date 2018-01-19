using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class PieceDataRepository : Repository<PieceData>, IPieceDataRepository
    {
        public PieceDataRepository(StormContext context) : base(context)
        {

        }

        public StormContext StormContext => Context as StormContext;
    }
}
