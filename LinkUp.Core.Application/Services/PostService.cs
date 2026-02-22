using AutoMapper;
using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;
using Microsoft.AspNetCore.Hosting;

namespace LinkUp.Core.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public PostService(IPostRepository postRepository, IMapper mapper, IWebHostEnvironment env)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _env = env;
        }

        public async Task<(bool ok, string? error)> CreateAsync(string currentUserId, CreatePostDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Content)) return (false, "El contenido es requerido");
            if (dto.Content.Length > 1000) return (false, "El contenido no puede exceder 1000 caracteres");

            if (dto.MediaKind == "image")
            {
                if (string.IsNullOrWhiteSpace(dto.SavedImagePath)) return (false, "Debe subir una imagen");
            }
            else if (dto.MediaKind == "youtube")
            {
                if (string.IsNullOrWhiteSpace(dto.YouTubeUrl)) return (false, "Debe especificar un enlace de YouTube");
                if (!IsValidYouTubeUrl(dto.YouTubeUrl!)) return (false, "Enlace de YouTube inválido");
            }
            else
            {
                return (false, "Tipo de medio inválido");
            }

            var post = new Post
            {
                UserId = currentUserId,
                CreatedBy = currentUserId,
                Content = dto.Content,
                MediaType = dto.MediaKind == "image" ? MediaType.Image : MediaType.YouTube,
                ImageUrl = dto.SavedImagePath,
                YouTubeUrl = dto.YouTubeUrl
            };

            await _postRepository.AddAsync(post);
            return (true, null);
        }

        private static bool IsValidYouTubeUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;
            var host = uri.Host.ToLowerInvariant();
            if (!(host.Contains("youtube.com") || host.Contains("youtu.be"))) return false;
            return true;
        }

        public async Task<(bool ok, string? error)> UpdateAsync(string currentUserId, EditPostDto dto)
        {
            var post = await _postRepository.GetByIdAsync(dto.Id);
            if (post == null) return (false, "Publicación no encontrada");
            if (post.UserId != currentUserId) return (false, "No autorizado");
            if (string.IsNullOrWhiteSpace(dto.Content)) return (false, "El contenido es requerido");
            if (dto.Content.Length > 1000) return (false, "El contenido no puede exceder 1000 caracteres");
            post.Content = dto.Content;
            post.UpdatedBy = currentUserId;
            post.UpdatedAt = DateTime.UtcNow;
            await _postRepository.UpdateAsync(post);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(string currentUserId, int postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return (false, "Publicación no encontrada");
            if (post.UserId != currentUserId) return (false, "No autorizado");
            post.UpdatedBy = currentUserId;
            post.UpdatedAt = DateTime.UtcNow;
            await _postRepository.DeleteAsync(post); // repository performs soft-delete + cascade soft-delete related
            return (true, null);
        }

        public async Task<IReadOnlyList<PostDto>> GetMyPostsAsync(string currentUserId)
        {
            var posts = await _postRepository.GetByUserAsync(currentUserId);
            var dtos = _mapper.Map<List<PostDto>>(posts);
            // Asignar la reacción del usuario actual para cada post
            for (int i = 0; i < dtos.Count; i++)
            {
                dtos[i].MyReaction = posts[i].Reactions.FirstOrDefault(r => r.UserId == currentUserId)?.Type;
            }
            return dtos;
        }
    }
}
