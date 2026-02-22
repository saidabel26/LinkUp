using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Domain.Interfaces.Persistence
{
    public interface IPostRepository
    {
        Task<IReadOnlyList<Post>> GetByUserAsync(string userId);
        Task<IReadOnlyList<Post>> GetByUsersAsync(IEnumerable<string> userIds);
        Task<Post?> GetByIdAsync(int id);
        Task<Post> AddAsync(Post post);
        Task UpdateAsync(Post post);
        Task DeleteAsync(Post post);
    }
}
