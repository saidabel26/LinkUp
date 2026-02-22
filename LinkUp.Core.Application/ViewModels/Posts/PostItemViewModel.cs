using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Application.ViewModels.Posts
{
    public class CommentItemViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AuthorUserName { get; set; } = string.Empty;
        public string? AuthorProfilePhotoUrl { get; set; }
        public List<CommentItemViewModel> Replies { get; set; } = new();
    }

    public class PostItemViewModel
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string AuthorUserName { get; set; } = string.Empty;
        public string? AuthorProfilePhotoUrl { get; set; }
        public MediaType MediaType { get; set; }
        public string? ImageUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public ReactionType? MyReaction { get; set; }
        public List<CommentItemViewModel> Comments { get; set; } = new();
    }
}
