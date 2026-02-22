using LinkUp.Core.Application.Dtos.Identity;
using LinkUp.Infrastructure.Identity.Contexts;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Identity.Services
{
    public class IdentityReadService : LinkUp.Core.Application.Interfaces.IIdentityReadService
    {
        private readonly IdentityContext _ctx;
        public IdentityReadService(IdentityContext ctx)
        {
            _ctx = ctx;
        }

        public async Task<IReadOnlyList<UserProfileDto>> GetByIdsAsync(IEnumerable<string> ids)
        {
            var set = ids.ToHashSet();
            return await _ctx.Set<AppUser>()
                .Where(u => set.Contains(u.Id))
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    UserName = u.UserName!,
                    FullName = u.FirstName + " " + u.LastName,
                    ProfilePhotoUrl = u.ProfilePhotoUrl
                }).ToListAsync();
        }

        public async Task<IReadOnlyList<UserProfileDto>> SearchActiveUsersAsync(string? query, int take = 50)
        {
            var q = _ctx.Set<AppUser>().Where(u => u.EmailConfirmed);
            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(u => u.UserName!.Contains(query));
            }
            return await q.OrderBy(u => u.UserName)
                .Take(take)
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    UserName = u.UserName!,
                    FullName = u.FirstName + " " + u.LastName,
                    ProfilePhotoUrl = u.ProfilePhotoUrl
                }).ToListAsync();
        }
    }
}
