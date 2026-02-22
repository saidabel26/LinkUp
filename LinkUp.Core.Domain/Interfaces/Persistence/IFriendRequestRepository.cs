using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Domain.Interfaces.Persistence
{
    public interface IFriendRequestRepository
    {
        Task<IReadOnlyList<FriendRequest>> GetIncomingAsync(string userId);
        Task<IReadOnlyList<FriendRequest>> GetOutgoingAsync(string userId);
        Task<FriendRequest?> GetAsync(string fromUserId, string toUserId);
        Task<FriendRequest?> GetIncludingDeletedAsync(string fromUserId, string toUserId);
        Task<FriendRequest?> GetByIdAsync(int id);
        Task<FriendRequest> AddAsync(FriendRequest request);
        Task UpdateAsync(FriendRequest request);
        Task DeleteAsync(FriendRequest request);
    }
}
