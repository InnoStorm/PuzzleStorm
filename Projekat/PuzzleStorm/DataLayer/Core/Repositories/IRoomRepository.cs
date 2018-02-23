using System.Collections.Generic;
using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        IEnumerable<Room> GetAllAvailable();
        IEnumerable<Room> GetAllPlaying();
    }
}
