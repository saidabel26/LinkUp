using System.ComponentModel.DataAnnotations;
using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Domain.Social
{
    public class Post : Common.BaseEntity
    {
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
        public MediaType MediaType { get; set; }
        public string? ImageUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
