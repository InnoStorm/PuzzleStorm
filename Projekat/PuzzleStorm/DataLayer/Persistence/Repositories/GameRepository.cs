using System.Collections.Generic;
using System.Linq;
using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;
using System.Data.Entity;
using StormCommonData.Enums;

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
