using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}
