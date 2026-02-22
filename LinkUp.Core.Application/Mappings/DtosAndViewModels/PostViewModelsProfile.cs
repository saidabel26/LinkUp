using LinkUp.Core.Application.ViewModels.Posts;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Social;
using AutoMapper;

namespace LinkUp.Core.Application.Mappings.DtosAndViewModels
{
    public class PostViewModelsProfile : Profile
    {
        public PostViewModelsProfile()
        {
            CreateMap<CreatePostViewModel, Post>()
                .ForMember(d => d.MediaType, opt => opt.MapFrom(src => src.MediaKind == "image" ? MediaType.Image : MediaType.YouTube))
                .ForMember(d => d.ImageUrl, opt => opt.Ignore())
                .ForMember(d => d.YouTubeUrl, opt => opt.MapFrom(src => src.YouTubeUrl));
        }
    }
}
