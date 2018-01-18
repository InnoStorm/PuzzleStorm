using System.Linq;
using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;
using System.Data.Entity;

namespace DataLayer.Persistence.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(StormContext context) : base(context)
        {

        }
        
        public Player GetPlayerWithUser(int id)
        {
            return StormContext.Players
                .Include(p => p.UserForPlayer)
                .Single(p => p.Id == id);
        }

        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
