using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Player GetPlayerWithUser(int id);
    }
}
