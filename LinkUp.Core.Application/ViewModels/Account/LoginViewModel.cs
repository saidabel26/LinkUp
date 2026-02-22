using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public string? ErrorMessage { get; set; }
        public bool IsInactive { get; set; }
    }
}
