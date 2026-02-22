using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Posts
{
    public class CreatePostViewModel
    {
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
        [Required]
        public string MediaKind { get; set; } = "image"; // image or youtube
        public IFormFile? Image { get; set; }
        public string? YouTubeUrl { get; set; }
    }
}
