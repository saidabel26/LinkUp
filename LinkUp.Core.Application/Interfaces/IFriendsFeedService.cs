namespace LinkUp.Core.Application.Interfaces
{
    using LinkUp.Core.Application.Dtos.Social;

    public interface IFriendsFeedService
    {
        Task<IReadOnlyList<PostDto>> GetFriendsPostsAsync(string currentUserId);
        Task<IReadOnlyList<(string userId, string userName, string? photo)>> GetFriendsAsync(string currentUserId);
    }
}
