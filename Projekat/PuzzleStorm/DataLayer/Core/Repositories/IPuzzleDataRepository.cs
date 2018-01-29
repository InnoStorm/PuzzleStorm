using DataLayer.Core.Domain;
using StormCommonData.Enums;

namespace DataLayer.Core.Repositories
{
    public interface IPuzzleDataRepository : IRepository<PuzzleData>
    {
        PuzzleData GetPuzzle(int numberOfPieces);
    }
}
