using System.Collections.Generic;
using System.Linq;
using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;
using System.Data.Entity;
using StormCommonData.Enums;

namespace DataLayer.Persistence.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(StormContext context) : base(context)
        {

        }
        
        public StormContext StormContext => Context as StormContext;

        public IEnumerable<Room> GetAllAvailable()
        {
            return Find(x => x.State == RoomState.Available);
        }
    }
}
