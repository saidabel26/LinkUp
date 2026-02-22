using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        public string Token { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string? Error { get; set; }
        public string? Message { get; set; }
    }
}
