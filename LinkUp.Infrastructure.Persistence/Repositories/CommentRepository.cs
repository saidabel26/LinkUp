using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;
        public CommentRepository(AppDbContext context) { _context = context; }

        public async Task<Comment> AddAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        public async Task DeleteAsync(Comment comment)
        {
            // Soft delete parent and recursively soft delete all descendants
            var now = DateTime.UtcNow;
            comment.IsDeleted = true;
            comment.UpdatedAt = now;
            _context.Comments.Update(comment);

            var queue = new Queue<int>();
            queue.Enqueue(comment.Id);
            while (queue.Count > 0)
            {
                var parentId = queue.Dequeue();
                var children = await _context.Comments
                    .Where(c => c.ParentCommentId == parentId && !c.IsDeleted)
                    .ToListAsync();
                foreach (var child in children)
                {
                    child.IsDeleted = true;
                    child.UpdatedAt = now;
                    child.UpdatedBy = comment.UpdatedBy;
                    _context.Comments.Update(child);
                    queue.Enqueue(child.Id);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Comment?> GetByIdAsync(int id)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Comment comment)
        {
            _context.Comments.Update(comment);
            await _context.SaveChangesAsync();
        }
    }
}
