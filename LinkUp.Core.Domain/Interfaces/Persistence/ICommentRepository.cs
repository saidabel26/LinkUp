using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Domain.Interfaces.Persistence
{
    public interface ICommentRepository
    {
        Task<Comment> AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(int id);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}
