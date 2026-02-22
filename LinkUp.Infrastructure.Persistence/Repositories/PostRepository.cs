using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly AppDbContext _context;
        public PostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Post> AddAsync(Post post)
        {
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task DeleteAsync(Post post)
        {
            // Eliminación suave de la publicación y entidades relacionadas
            var now = DateTime.UtcNow;

            post.IsDeleted = true;
            post.UpdatedAt = now;
            _context.Posts.Update(post);

            // Eliminar temporalmente todos los comentarios de esta publicación (incluidas las respuestas)
            var comments = await _context.Comments
                .Where(c => c.PostId == post.Id && !c.IsDeleted)
                .ToListAsync();
            foreach (var c in comments)
            {
                c.IsDeleted = true;
                c.UpdatedAt = now;
                c.UpdatedBy = post.UpdatedBy;
                _context.Comments.Update(c);
            }

            // Soft delete de todas las reacciones para esta publicación
            var reactions = await _context.Reactions
                .Where(r => r.PostId == post.Id && !r.IsDeleted)
                .ToListAsync();
            foreach (var r in reactions)
            {
                r.IsDeleted = true;
                r.UpdatedAt = now;
                r.UpdatedBy = post.UpdatedBy;
                _context.Reactions.Update(r);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Post?> GetByIdAsync(int id)
        {
            return await _context.Posts
                .Include(p => p.Reactions)
                .Include(p => p.Comments)
                    .ThenInclude(c => c.Replies)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Post>> GetByUserAsync(string userId)
        {
            return await _context.Posts
                .Where(p => p.UserId == userId)
                .Include(p => p.Reactions)
                .Include(p => p.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Post>> GetByUsersAsync(IEnumerable<string> userIds)
        {
            var set = userIds.ToHashSet();
            return await _context.Posts
                .Where(p => set.Contains(p.UserId))
                .Include(p => p.Reactions)
                .Include(p => p.Comments.Where(c => c.ParentCommentId == null))
                    .ThenInclude(c => c.Replies)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(Post post)
        {
            _context.Posts.Update(post);
            await _context.SaveChangesAsync();
        }
    }
}
