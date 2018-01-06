using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Core;
using Server.Core.Repositories;
using Server.Persistence.Repositories;

namespace Server.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StormContext _context;

        public UnitOfWork(StormContext context)
        {
            _context = context;

            Games = new GameRepository(_context);
            Pieces = new PieceRepository(_context);
            Players = new PlayerRepository(_context);
            Puzzles = new PuzzleRepository(_context);
            RoomProperties = new RoomPropertiesRepository(_context);
            Rooms = new RoomRepository(_context);
            Users = new UserRepository(_context);
        }
        
        public IGameRepository Games { get; private set; }
        public IPieceRepository Pieces { get; private set; }
        public IPlayerRepository Players { get; private set; }
        public IPuzzleRepository Puzzles { get; private set; }
        public IRoomPropertiesRepository RoomProperties { get; private set; }
        public IRoomRepository Rooms { get; private set; }
        public IUserRepository Users { get; private set; }

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
