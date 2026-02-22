using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Profile
{
    public class EditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [RegularExpression(@"^(\+?1)?(809|829|849)[0-9]{7}$", ErrorMessage = "Formato de teléfono RD inválido")]
        public string Phone { get; set; } = string.Empty;
        public IFormFile? ProfilePhoto { get; set; }
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}
