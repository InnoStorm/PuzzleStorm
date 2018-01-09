using System.Data.Entity;
using System.Linq;
using DataLayer.Core.Domain;
using DataLayer.Core.Repositories;

namespace DataLayer.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(StormContext context) : base(context)
        {

        }

        public User FindByUsername(string username)
        {
            return StormContext.Users.Where(u => u.Username == username).Single();
        }

        public User GetUserWithPlayer(string username)
        {
            return StormContext.Users.Include(u => u.PlayerForUser).Single(u => u.Username == username);
        }

        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
