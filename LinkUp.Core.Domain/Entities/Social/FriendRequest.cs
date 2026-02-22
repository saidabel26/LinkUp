using System;
using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Domain.Social
{
    public class FriendRequest : Common.BaseEntity
    {
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;
        public DateTime? RespondedAt { get; set; }
    }
}
