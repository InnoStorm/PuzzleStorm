using DataLayer.Core.Domain;

namespace DataLayer.Core.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        User FindByUsername(string userName);
    }
}
