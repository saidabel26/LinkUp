using LinkUp.Core.Application.Dtos.Social;

namespace LinkUp.Core.Application.Interfaces
{
    public interface ICommentService
    {
        Task<(bool ok, string? error)> AddCommentAsync(int postId, string userId, string content);
        Task<(bool ok, string? error)> AddReplyAsync(int parentCommentId, string userId, string content);
        Task<(bool ok, string? error)> EditCommentAsync(int commentId, string userId, string content);
        Task<(bool ok, string? error)> DeleteCommentAsync(int commentId, string userId);
    }
}
