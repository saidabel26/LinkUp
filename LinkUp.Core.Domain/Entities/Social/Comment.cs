using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Domain.Social
{
    public class Comment : Common.BaseEntity
    {
        [Required]
        public int PostId { get; set; }
        public Post? Post { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        [Required]
        [MaxLength(750)]
        public string Content { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
