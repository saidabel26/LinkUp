using System.ComponentModel.DataAnnotations;
using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Domain.Social
{
    public class Reaction : Common.BaseEntity
    {
        [Required]
        public int PostId { get; set; }
        public Post? Post { get; set; }
        [Required]
        public string UserId { get; set; } = string.Empty;
        public ReactionType Type { get; set; }
    }
}
