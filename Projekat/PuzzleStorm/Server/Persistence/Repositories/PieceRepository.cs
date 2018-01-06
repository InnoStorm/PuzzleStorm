using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core.Domain;
using Server.Core.Repositories;

namespace Server.Persistence.Repositories
{
    public class PieceRepository : Repository<Piece>, IPieceRepository
    {
        public PieceRepository(StormContext context) : base(context)
        {

        }





        public StormContext StormContext
        {
            get { return Context as StormContext; }
        }
    }
}
