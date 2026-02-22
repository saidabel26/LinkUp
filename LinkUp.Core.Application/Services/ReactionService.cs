using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Application.Services
{
    public class ReactionService : IReactionService
    {
        private readonly IReactionRepository _reactionRepository;
        private readonly IPostRepository _postRepository;

        public ReactionService(IReactionRepository reactionRepository, IPostRepository postRepository)
        {
            _reactionRepository = reactionRepository;
            _postRepository = postRepository;
        }

        public async Task<(bool ok, string? error)> ReactAsync(int postId, string userId, ReactionType type)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null) return (false, "Publicación no encontrada");

            // Buscar incluyendo eliminadas para evitar romper el índice único al recrear
            var existingAny = await _reactionRepository.GetByPostAndUserIncludingDeletedAsync(postId, userId);
            if (existingAny == null)
            {
                await _reactionRepository.AddAsync(new Reaction { PostId = postId, UserId = userId, CreatedBy = userId, Type = type });
                return (true, null);
            }

            if (existingAny.IsDeleted)
            {
                existingAny.IsDeleted = false;
                existingAny.Type = type;
                existingAny.UpdatedBy = userId;
                existingAny.UpdatedAt = DateTime.UtcNow;
                await _reactionRepository.UpdateAsync(existingAny);
                return (true, null);
            }

            if (existingAny.Type == type)
            {
                return (true, null);
            }

            existingAny.Type = type;
            existingAny.UpdatedBy = userId;
            existingAny.UpdatedAt = DateTime.UtcNow;
            await _reactionRepository.UpdateAsync(existingAny);
            return (true, null);
        }
    }
}
