using LinkUp.Core.Application.Dtos.Identity;
using LinkUp.Core.Application.Dtos.Social;

namespace LinkUp.Core.Application.Interfaces
{
    public interface IFriendRequestsService
    {
        Task<(IReadOnlyList<FriendRequestDto> incoming, IReadOnlyList<FriendRequestDto> outgoing)> GetListsAsync(string currentUserId);
        Task<(bool ok, string? error)> AcceptAsync(int requestId, string currentUserId);
        Task<(bool ok, string? error)> RejectAsync(int requestId, string currentUserId);
        Task<(bool ok, string? error)> DeleteAsync(int requestId, string currentUserId);
        Task<(bool ok, string? error)> CreateAsync(string currentUserId, string toUserId);
        Task<IReadOnlyList<EligibleUserDto>> GetEligibleUsersAsync(string currentUserId, string? search);
    }
}
