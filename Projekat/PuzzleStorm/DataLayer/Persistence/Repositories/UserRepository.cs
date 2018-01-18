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
            return StormContext.Users
                .Single(u => u.Username == username);
        }

        public bool UsernameExists(string username)
        {
            return StormContext.Users
                .Any(x => x.Username == username);
        }

        public User GetUserWithPlayer(string username)
        {
            return StormContext.Users
                .Include(u => u.PlayerForUser)
                .Single(u => u.Username == username);
        }

        public void MakePlayerForUser(int id)
        {
            var player = new Player();
            var user = Get(id);
            user.PlayerForUser = player;
        }

        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
