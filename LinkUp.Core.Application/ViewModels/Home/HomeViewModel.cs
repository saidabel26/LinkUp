using LinkUp.Core.Application.ViewModels.Posts;

namespace LinkUp.Core.Application.ViewModels.Home
{
    public class HomeViewModel
    {
        public CreatePostViewModel Create { get; set; } = new();
        public List<PostItemViewModel> Posts { get; set; } = new();
    }
}
