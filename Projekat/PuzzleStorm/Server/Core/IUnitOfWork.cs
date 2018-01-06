using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core.Repositories;

namespace Server.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGameRepository Games { get; }
        IPieceRepository Pieces { get; }
        IPlayerRepository Players { get; }
        IPuzzleRepository Puzzles { get; }
        IRoomPropertiesRepository RoomProperties { get; }
        IRoomRepository Rooms { get; }
        IUserRepository Users { get; }

        int Complete();
    }
}
