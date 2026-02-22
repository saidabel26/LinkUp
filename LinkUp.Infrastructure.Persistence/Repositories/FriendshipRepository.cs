using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly AppDbContext _ctx;
        public FriendshipRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task<Friendship> AddAsync(Friendship friendship)
        {
            _ctx.Friendships.Add(friendship);
            await _ctx.SaveChangesAsync();
            return friendship;
        }

        public async Task<bool> AreFriendsAsync(string userId, string friendId)
        {
            return await _ctx.Friendships.AnyAsync(f => !f.IsDeleted && ((f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId)));
        }

        public async Task<Friendship?> FindAsync(string userId, string friendId)
        {
            return await _ctx.Friendships.FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
        }

        public async Task<Friendship?> FindIncludingDeletedAsync(string userId, string friendId)
        {
            return await _ctx.Friendships.IgnoreQueryFilters().FirstOrDefaultAsync(f => (f.UserId == userId && f.FriendId == friendId) || (f.UserId == friendId && f.FriendId == userId));
        }

        public async Task<IReadOnlyList<Friendship>> GetFriendsAsync(string userId)
        {
            return await _ctx.Friendships.Where(f => (f.UserId == userId || f.FriendId == userId)).ToListAsync();
        }

        public async Task RemoveAsync(Friendship friendship)
        {
            friendship.IsDeleted = true;
            await _ctx.SaveChangesAsync();
        }

        public async Task UpdateAsync(Friendship friendship)
        {
            _ctx.Friendships.Update(friendship);
            await _ctx.SaveChangesAsync();
        }
    }
}
