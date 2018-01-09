using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(StormContext context) : base(context)
        {

        }





        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
