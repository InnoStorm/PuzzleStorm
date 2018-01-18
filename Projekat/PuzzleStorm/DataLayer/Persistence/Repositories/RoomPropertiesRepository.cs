using System.Data.Entity;
using System.Linq;
using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class RoomPropertiesRepository : Repository<RoomProperties>, IRoomPropertiesRepository
    {
        public RoomPropertiesRepository(StormContext context) : base(context)
        {

        }

        public RoomProperties GetPropertiesOfRoom(int id)
        {
            return StormContext.RoomProperties.Single(r => r.RoomOfThisProperties.Id == id);
        }
        
        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
