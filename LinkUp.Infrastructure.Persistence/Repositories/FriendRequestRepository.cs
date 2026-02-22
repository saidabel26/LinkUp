using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly AppDbContext _ctx;
        public FriendRequestRepository(AppDbContext ctx) { _ctx = ctx; }

        public async Task<FriendRequest> AddAsync(FriendRequest request)
        {
            _ctx.FriendRequests.Add(request);
            await _ctx.SaveChangesAsync();
            return request;
        }

        public async Task DeleteAsync(FriendRequest request)
        {
            // Soft delete
            request.IsDeleted = true;
            _ctx.FriendRequests.Update(request);
            await _ctx.SaveChangesAsync();
        }

        public async Task<FriendRequest?> GetAsync(string fromUserId, string toUserId)
        {
            return await _ctx.FriendRequests.FirstOrDefaultAsync(fr => fr.FromUserId == fromUserId && fr.ToUserId == toUserId);
        }

        public async Task<FriendRequest?> GetIncludingDeletedAsync(string fromUserId, string toUserId)
        {
            return await _ctx.FriendRequests.IgnoreQueryFilters().FirstOrDefaultAsync(fr => fr.FromUserId == fromUserId && fr.ToUserId == toUserId);
        }

        public async Task<FriendRequest?> GetByIdAsync(int id)
        {
            return await _ctx.FriendRequests.FirstOrDefaultAsync(fr => fr.Id == id);
        }

        public async Task<IReadOnlyList<FriendRequest>> GetIncomingAsync(string userId)
        {
            return await _ctx.FriendRequests
                .Where(fr => fr.ToUserId == userId && fr.Status == Core.Domain.Common.Enums.FriendRequestStatus.Pending)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<FriendRequest>> GetOutgoingAsync(string userId)
        {
            return await _ctx.FriendRequests
                .Where(fr => fr.FromUserId == userId)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(FriendRequest request)
        {
            _ctx.FriendRequests.Update(request);
            await _ctx.SaveChangesAsync();
        }
    }
}
