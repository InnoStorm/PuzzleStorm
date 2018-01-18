using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User FindByUsername(string userName);
        bool UsernameExists(string username);
        User GetUserWithPlayer(string username);
    }
}
