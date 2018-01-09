using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(StormContext context) : base(context)
        {

        }





        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
