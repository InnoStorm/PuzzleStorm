using System;
using DataLayer.Core.Repositories;

namespace DataLayer.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGameRepository Games { get; }
        IPieceDataRepository Pieces { get; }
        IPlayerRepository Players { get; }
        IPuzzleDataRepository Puzzles { get; }
        IRoomRepository Rooms { get; }

        int Complete();
    }
}
