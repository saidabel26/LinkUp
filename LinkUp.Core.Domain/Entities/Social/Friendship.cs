namespace LinkUp.Core.Domain.Social
{
    public class Friendship : Common.BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string FriendId { get; set; } = string.Empty;
    }
}
