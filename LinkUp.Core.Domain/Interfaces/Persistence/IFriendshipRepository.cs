using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Domain.Interfaces.Persistence
{
    public interface IFriendshipRepository
    {
        Task<IReadOnlyList<Friendship>> GetFriendsAsync(string userId);
        Task<bool> AreFriendsAsync(string userId, string friendId);
        Task<Friendship> AddAsync(Friendship friendship);
        Task RemoveAsync(Friendship friendship);
        Task<Friendship?> FindAsync(string userId, string friendId);
        Task<Friendship?> FindIncludingDeletedAsync(string userId, string friendId);
        Task UpdateAsync(Friendship friendship);
    }
}
