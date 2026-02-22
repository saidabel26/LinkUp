using System.ComponentModel.DataAnnotations;

namespace LinkUp.Core.Application.ViewModels.Posts
{
    public class CommentCreateViewModel
    {
        [Required]
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
        [Required]
        [MaxLength(750)]
        public string Content { get; set; } = string.Empty;
    }

    public class CommentEditViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(750)]
        public string Content { get; set; } = string.Empty;
    }
}
