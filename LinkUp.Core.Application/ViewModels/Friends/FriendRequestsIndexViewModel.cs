using LinkUp.Core.Application.Dtos.Identity;
using LinkUp.Core.Application.Dtos.Social;

namespace LinkUp.Core.Application.ViewModels.Friends
{
    public class FriendRequestsIndexViewModel
    {
        public List<FriendRequestDto> Incoming { get; set; } = new();
        public List<FriendRequestDto> Outgoing { get; set; } = new();
        public Dictionary<string, UserProfileDto> Profiles { get; set; } = new();
    }
}
