using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Application.Services
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly IIdentityReadService _identityReadService;
        private readonly IPostRepository _postRepository;

        public FriendsService(IFriendshipRepository friendshipRepository, IFriendRequestRepository friendRequestRepository, IIdentityReadService identityReadService, IPostRepository postRepository)
        {
            _friendshipRepository = friendshipRepository;
            _friendRequestRepository = friendRequestRepository;
            _identityReadService = identityReadService;
            _postRepository = postRepository;
        }

        public async Task<FriendsHomeSummaryDto> GetHomeSummaryAsync(string currentUserId)
        {
            var friends = await _friendshipRepository.GetFriendsAsync(currentUserId);
            var incoming = await _friendRequestRepository.GetIncomingAsync(currentUserId);
            return new FriendsHomeSummaryDto
            {
                FriendsCount = friends.Count,
                PendingRequestsCount = incoming.Count(fr => fr.Status == LinkUp.Core.Domain.Common.Enums.FriendRequestStatus.Pending)
            };
        }

        public async Task<(bool ok, string? error)> RemoveFriendAsync(string currentUserId, string friendId)
        {
            var friendship = await _friendshipRepository.FindAsync(currentUserId, friendId);
            if (friendship == null) return (false, "No existe la amistad");
            friendship.UpdatedBy = currentUserId;
            await _friendshipRepository.RemoveAsync(friendship);
            return (true, null);
        }
    }
}
