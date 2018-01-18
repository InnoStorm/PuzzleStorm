using System.Linq;
using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;
using System.Data.Entity;

namespace DataLayer.Persistence.Repositories
{
    public class RoomRepository : Repository<Room>, IRoomRepository
    {
        public RoomRepository(StormContext context) : base(context)
        {

        }

        public Room GetRoomIncludeAll(int id)
        {
            return StormContext.Rooms
                    .Include(r => r.Properties)
                    .Include(r => r.CurrentGame)
                    .Include(r => r.ListOfPlayers)
                    .Include(r => r.ListOfPuzzles)
                    .Single(r => r.Id == id);
        }
        
        public Room GetRoomWithProperties(int id)
        {
            return StormContext.Rooms
                    .Include(r => r.Properties)
                    .Single(r => r.Id == id);
        }

        public Room GetRoomWithPlayersAndProperties(int id)
        {
            return StormContext.Rooms
                    .Include(r => r.ListOfPlayers)
                    .Include(r => r.Properties)
                    .Single(r => r.Id == id);
        }

        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
