using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IRoomPropertiesRepository : IRepository<RoomProperties>
    {
        RoomProperties GetPropertiesOfRoom(int id);
    }
}
