using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Dtos.Social
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int? ParentCommentId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
    }

    public class PostDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public MediaType MediaType { get; set; }
        public string? ImageUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public ReactionType? MyReaction { get; set; }
        public List<CommentDto> Comments { get; set; } = new();
    }
}
