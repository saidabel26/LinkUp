namespace LinkUp.Core.Application.Dtos.Social
{
    public class FriendDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
        public int CommonFriendsCount { get; set; }
    }
}
