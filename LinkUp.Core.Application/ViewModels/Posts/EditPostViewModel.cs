using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Posts
{
    public class EditPostViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}
