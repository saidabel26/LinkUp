namespace LinkUp.Core.Application.Dtos.Identity
{
    public class UserProfileDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }
        public string? FullName { get; set; }
    }
}
