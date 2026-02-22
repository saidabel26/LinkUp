using AutoMapper;
using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels
{
    public class SocialDtosProfile : Profile
    {
        public SocialDtosProfile()
        {
            // Domain -> DTOs
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.Replies, opt => opt.MapFrom(s => s.Replies));

            CreateMap<Post, PostDto>()
                .ForMember(d => d.Comments, opt => opt.MapFrom(s => s.Comments.Where(c => c.ParentCommentId == null)))
                .ForMember(d => d.MyReaction, opt => opt.Ignore());

            // ViewModels -> Command DTOs
            CreateMap<ViewModels.Posts.EditPostViewModel, EditPostDto>();
        }
    }
}
