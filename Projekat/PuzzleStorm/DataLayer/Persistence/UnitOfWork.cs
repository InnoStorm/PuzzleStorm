using DataLayer.Core;
using DataLayer.Core.Repositories;
using DataLayer.Persistence.Repositories;

namespace DataLayer.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StormContext _context;

        public UnitOfWork(StormContext context)
        {
            _context = context;

            Games = new GameRepository(_context);
            Pieces = new PieceDataRepository(_context);
            Players = new PlayerRepository(_context);
            Puzzles = new PuzzleDataRepository(_context);
            Rooms = new RoomRepository(_context);
        }
        
        public IGameRepository Games { get; private set; }
        public IPieceDataRepository Pieces { get; private set; }
        public IPlayerRepository Players { get; private set; }
        public IPuzzleDataRepository Puzzles { get; private set; }
        public IRoomRepository Rooms { get; private set; }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
