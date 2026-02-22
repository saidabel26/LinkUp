using LinkUp.Core.Domain.Battleship;

namespace LinkUp.Core.Domain.Interfaces.Persistence
{
    public interface IBattleshipGameRepository
    {
        Task<BattleshipGame?> GetByIdAsync(int id);
        Task<BattleshipGame> AddAsync(BattleshipGame game);
        Task UpdateAsync(BattleshipGame game);
        Task<bool> ExistsActiveBetweenAsync(string userA, string userB);
        Task<IReadOnlyList<BattleshipGame>> GetActiveByUserAsync(string userId);
        Task<IReadOnlyList<BattleshipGame>> GetFinishedByUserAsync(string userId);

        // Posicionamientos
        Task<IReadOnlyList<ShipPlacement>> GetPlacementsAsync(int gameId, string userId);
        Task AddPlacementAsync(ShipPlacement placement);

        // Ataques
        Task<IReadOnlyList<Attack>> GetAttacksAsync(int gameId, string userId);
        Task<bool> HasAttackAtAsync(int gameId, string userId, int row, int col);
        Task AddAttackAsync(Attack attack);

        // Proceso de abandono por tiempo
        Task<IReadOnlyList<int>> GetPotentiallyTimedOutGameIdsAsync(DateTime thresholdUtc);
    }
}
