using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        Room GetRoomIncludeAll(int id);
        Room GetRoomWithProperties(int id);
        Room GetRoomWithPlayersAndProperties(int id);
    }
}
