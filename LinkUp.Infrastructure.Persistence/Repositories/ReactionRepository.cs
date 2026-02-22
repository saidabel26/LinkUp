using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class ReactionRepository : IReactionRepository
    {
        private readonly AppDbContext _context;
        public ReactionRepository(AppDbContext context) { _context = context; }

        public async Task<Reaction> AddAsync(Reaction reaction)
        {
            _context.Reactions.Add(reaction);
            await _context.SaveChangesAsync();
            return reaction;
        }

        public async Task DeleteAsync(Reaction reaction)
        {
            // Soft delete reaction
            reaction.IsDeleted = true;
            reaction.UpdatedAt = DateTime.UtcNow;
            _context.Reactions.Update(reaction);
            await _context.SaveChangesAsync();
        }

        public async Task<Reaction?> GetByPostAndUserAsync(int postId, string userId)
        {
            return await _context.Reactions.FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
        }

        public async Task<Reaction?> GetByPostAndUserIncludingDeletedAsync(int postId, string userId)
        {
            return await _context.Reactions.IgnoreQueryFilters().FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId);
        }

        public async Task UpdateAsync(Reaction reaction)
        {
            _context.Reactions.Update(reaction);
            await _context.SaveChangesAsync();
        }
    }
}
