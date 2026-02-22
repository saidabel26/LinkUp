using LinkUp.Core.Application.Dtos.Identity;

namespace LinkUp.Core.Application.Interfaces
{
    public interface IIdentityReadService
    {
        Task<IReadOnlyList<UserProfileDto>> GetByIdsAsync(IEnumerable<string> ids);
        Task<IReadOnlyList<UserProfileDto>> SearchActiveUsersAsync(string? query, int take = 50);
    }
}
