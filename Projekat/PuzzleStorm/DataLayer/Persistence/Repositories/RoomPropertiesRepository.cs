using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class RoomPropertiesRepository : Repository<RoomProperties>, IRoomPropertiesRepository
    {
        public RoomPropertiesRepository(StormContext context) : base(context)
        {

        }





        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
