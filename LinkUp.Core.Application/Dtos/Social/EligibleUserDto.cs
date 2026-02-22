namespace LinkUp.Core.Application.Dtos.Social
{
    public class EligibleUserDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public int CommonFriendsCount { get; set; }
    }
}
