using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Interfaces
{
    public interface IReactionService
    {
        Task<(bool ok, string? error)> ReactAsync(int postId, string userId, ReactionType type);
    }
}
