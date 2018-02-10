using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class GameRepository : Repository<Game>, IGameRepository
    {
        public GameRepository(StormContext context) : base(context)
        {
                
        }

        public StormContext StormContext => Context as StormContext;
    }
}
