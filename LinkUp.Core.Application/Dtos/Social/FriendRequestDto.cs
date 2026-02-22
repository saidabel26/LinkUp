using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Dtos.Social
{
    public class FriendRequestDto
    {
        public int Id { get; set; }
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public FriendRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }
        public int CommonFriendsCount { get; set; }
    }
}
