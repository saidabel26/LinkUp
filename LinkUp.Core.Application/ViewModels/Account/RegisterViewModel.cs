using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace LinkUp.Core.Application.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [Phone]
        [RegularExpression(@"^(\+?1)?(809|829|849)[0-9]{7}$", ErrorMessage = "Formato de teléfono RD inválido")]
        public string Phone { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public IFormFile? ProfilePhoto { get; set; }
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [RegularExpression(@"^(?=.*[a-z]).+$", ErrorMessage = "La contraseña debe tener al menos una letra minúscula")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? Error { get; set; }
        public string? Success { get; set; }
    }
}
