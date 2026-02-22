using LinkUp.Core.Domain.Battleship;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class BattleshipGameRepository : IBattleshipGameRepository
    {
        private readonly AppDbContext _ctx;
        public BattleshipGameRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task<BattleshipGame> AddAsync(BattleshipGame game)
        {
            _ctx.BattleshipGames.Add(game);
            await _ctx.SaveChangesAsync();
            return game;
        }

        public async Task<bool> ExistsActiveBetweenAsync(string userA, string userB)
        {
            return await _ctx.BattleshipGames.AnyAsync(g =>
                (g.Status == GameStatus.Active || g.Status == GameStatus.Setup) &&
                ((g.CreatorUserId == userA && g.OpponentUserId == userB) || (g.CreatorUserId == userB && g.OpponentUserId == userA))
            );
        }

        public async Task<IReadOnlyList<BattleshipGame>> GetActiveByUserAsync(string userId)
        {
            return await _ctx.BattleshipGames
                .Where(g => (g.Status == GameStatus.Active || g.Status == GameStatus.Setup) && (g.CreatorUserId == userId || g.OpponentUserId == userId))
                .ToListAsync();
        }

        public async Task<BattleshipGame?> GetByIdAsync(int id)
        {
            return await _ctx.BattleshipGames
                .Include(g => g.Placements)
                .Include(g => g.Attacks)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IReadOnlyList<BattleshipGame>> GetFinishedByUserAsync(string userId)
        {
            return await _ctx.BattleshipGames.Where(g => g.Status == GameStatus.Finished && (g.CreatorUserId == userId || g.OpponentUserId == userId)).ToListAsync();
        }

        public async Task UpdateAsync(BattleshipGame game)
        {
            _ctx.BattleshipGames.Update(game);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<ShipPlacement>> GetPlacementsAsync(int gameId, string userId)
        {
            return await _ctx.ShipPlacements.Where(p => p.GameId == gameId && p.UserId == userId).ToListAsync();
        }

        public async Task AddPlacementAsync(ShipPlacement placement)
        {
            _ctx.ShipPlacements.Add(placement);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Attack>> GetAttacksAsync(int gameId, string userId)
        {
            return await _ctx.Attacks.Where(a => a.GameId == gameId && a.AttackerUserId == userId).ToListAsync();
        }

        public async Task<bool> HasAttackAtAsync(int gameId, string userId, int row, int col)
        {
            return await _ctx.Attacks.AnyAsync(a => a.GameId == gameId && a.AttackerUserId == userId && a.Row == row && a.Col == col);
        }

        public async Task AddAttackAsync(Attack attack)
        {
            _ctx.Attacks.Add(attack);
            await _ctx.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<int>> GetPotentiallyTimedOutGameIdsAsync(DateTime thresholdUtc)
        {
            return await _ctx.BattleshipGames
                .Where(g => g.Status == GameStatus.Active && g.UpdatedAt.HasValue && g.UpdatedAt.Value < thresholdUtc)
                .Select(g => g.Id)
                .ToListAsync();
        }
    }
}
