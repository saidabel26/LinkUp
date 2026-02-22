using Microsoft.AspNetCore.Identity;

namespace LinkUp.Infrastructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePhotoUrl { get; set; }

        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
