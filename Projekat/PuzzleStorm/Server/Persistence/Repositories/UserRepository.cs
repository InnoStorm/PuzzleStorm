using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core.Domain;
using Server.Core.Repositories;

namespace Server.Persistence.Repositories
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
