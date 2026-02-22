using AutoMapper;
using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Services
{
    public class FriendsFeedService : IFriendsFeedService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IPostRepository _postRepository;
        private readonly IIdentityReadService _identityReadService;
        private readonly IMapper _mapper;

        public FriendsFeedService(IFriendshipRepository friendshipRepository, IPostRepository postRepository, IIdentityReadService identityReadService, IMapper mapper)
        {
            _friendshipRepository = friendshipRepository;
            _postRepository = postRepository;
            _identityReadService = identityReadService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<PostDto>> GetFriendsPostsAsync(string currentUserId)
        {
            var friends = await _friendshipRepository.GetFriendsAsync(currentUserId);
            var ids = friends.Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId).ToHashSet();
            var posts = await _postRepository.GetByUsersAsync(ids);
            var dtos = _mapper.Map<List<PostDto>>(posts);
            for (int i = 0; i < dtos.Count; i++)
            {
                dtos[i].MyReaction = posts[i].Reactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type;
            }
            return dtos;
        }

        public async Task<IReadOnlyList<(string userId, string userName, string? photo)>> GetFriendsAsync(string currentUserId)
        {
            var friends = await _friendshipRepository.GetFriendsAsync(currentUserId);
            var ids = friends.Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId).ToHashSet();
            var profiles = await _identityReadService.GetByIdsAsync(ids);
            return profiles.Select(p => (p.UserId, p.UserName, p.ProfilePhotoUrl)).ToList();
        }
    }
}
