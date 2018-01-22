using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IPlayerRepository : IRepository<Player>
    {
        Player Get(string username);
        bool IsUsernameAvailable(string username);
    }
}
