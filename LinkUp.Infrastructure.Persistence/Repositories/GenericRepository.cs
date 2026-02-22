using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        protected DbSet<T> Entities => _context.Set<T>();

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            Entities.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task DeleteAsync(T entity)
        {
            Entities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            return await Entities.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> ListAsync()
        {
            return await Entities.ToListAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            Entities.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
