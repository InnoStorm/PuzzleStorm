using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class PieceRepository : Repository<Piece>, IPieceRepository
    {
        public PieceRepository(StormContext context) : base(context)
        {

        }





        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
