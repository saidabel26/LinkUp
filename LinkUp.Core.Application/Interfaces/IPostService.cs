using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Interfaces
{
    public interface IPostService
    {
        Task<(bool ok, string? error)> CreateAsync(string currentUserId, CreatePostDto dto);
        Task<IReadOnlyList<PostDto>> GetMyPostsAsync(string currentUserId);
        Task<(bool ok, string? error)> UpdateAsync(string currentUserId, EditPostDto dto);
        Task<(bool ok, string? error)> DeleteAsync(string currentUserId, int postId);
    }
}
