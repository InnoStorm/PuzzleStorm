using System;
using DataLayer.Core.Repositories;

namespace DataLayer.Core
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
