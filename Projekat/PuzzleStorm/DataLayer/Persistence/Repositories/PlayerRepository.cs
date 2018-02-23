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

        public StormContext StormContext => Context as StormContext;

        public Player Get(string username)
        {
            return SingleOrDefault(x => x.Username == username);
        }

        public bool IsUsernameAvailable(string username)
        {
            return !StormContext.Players.Any(x => x.Username == username);
        }

    }
}
