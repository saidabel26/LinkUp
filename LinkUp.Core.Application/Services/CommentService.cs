using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;

        public CommentService(ICommentRepository commentRepository, IPostRepository postRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        public async Task<(bool ok, string? error)> AddCommentAsync(int postId, string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return (false, "El comentario es requerido");
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return (false, "Publicación no encontrada");
            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                CreatedBy = userId,
                Content = content
            };
            await _commentRepository.AddAsync(comment);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> AddReplyAsync(int parentCommentId, string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return (false, "El comentario es requerido");
            var parent = await _commentRepository.GetByIdAsync(parentCommentId);
            if (parent == null || parent.IsDeleted) return (false, "Comentario no encontrado");
            var reply = new Comment
            {
                PostId = parent.PostId,
                ParentCommentId = parentCommentId,
                UserId = userId,
                CreatedBy = userId,
                Content = content
            };
            await _commentRepository.AddAsync(reply);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> EditCommentAsync(int commentId, string userId, string content)
        {
            if (string.IsNullOrWhiteSpace(content)) return (false, "El comentario es requerido");
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.IsDeleted) return (false, "Comentario no encontrado");
            if (comment.UserId != userId) return (false, "No autorizado");
            comment.Content = content;
            comment.UpdatedBy = userId;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.UpdateAsync(comment);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteCommentAsync(int commentId, string userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null || comment.IsDeleted) return (false, "Comentario no encontrado");
            if (comment.UserId != userId) return (false, "No autorizado");
            comment.UpdatedBy = userId;
            comment.UpdatedAt = DateTime.UtcNow;
            await _commentRepository.DeleteAsync(comment);
            return (true, null);
        }
    }
}
