using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Domain.Interfaces.Persistence
{
    public interface IReactionRepository
    {
        Task<Reaction?> GetByPostAndUserAsync(int postId, string userId);
        Task<Reaction?> GetByPostAndUserIncludingDeletedAsync(int postId, string userId);
        Task<Reaction> AddAsync(Reaction reaction);
        Task UpdateAsync(Reaction reaction);
        Task DeleteAsync(Reaction reaction);
    }
}
