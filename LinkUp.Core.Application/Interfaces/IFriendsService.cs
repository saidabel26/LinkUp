namespace LinkUp.Core.Application.Interfaces
{
    public interface IFriendsService
    {
        Task<FriendsHomeSummaryDto> GetHomeSummaryAsync(string currentUserId);
        Task<(bool ok, string? error)> RemoveFriendAsync(string currentUserId, string friendId);
    }

    public class FriendsHomeSummaryDto
    {
        public int FriendsCount { get; set; }
        public int PendingRequestsCount { get; set; }
    }
}
