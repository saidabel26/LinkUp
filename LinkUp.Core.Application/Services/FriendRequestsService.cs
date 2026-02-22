using LinkUp.Core.Application.Dtos.Identity;
using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Core.Domain.Social;

namespace LinkUp.Core.Application.Services
{
    public class FriendRequestsService : IFriendRequestsService
    {
        private readonly IFriendRequestRepository _frRepo;
        private readonly IFriendshipRepository _fsRepo;
        private readonly IIdentityReadService _identityReadService;

        public FriendRequestsService(IFriendRequestRepository frRepo, IFriendshipRepository fsRepo, IIdentityReadService identityReadService)
        {
            _frRepo = frRepo;
            _fsRepo = fsRepo;
            _identityReadService = identityReadService;
        }

        public async Task<(IReadOnlyList<FriendRequestDto> incoming, IReadOnlyList<FriendRequestDto> outgoing)> GetListsAsync(string currentUserId)
        {
            var incoming = await _frRepo.GetIncomingAsync(currentUserId);
            var outgoing = await _frRepo.GetOutgoingAsync(currentUserId);

            var otherIds = incoming.Select(i => i.FromUserId).Concat(outgoing.Select(o => o.ToUserId)).ToHashSet();
            var commonCounts = await GetCommonFriendsCountsAsync(currentUserId, otherIds);

            IReadOnlyList<FriendRequestDto> Map(IEnumerable<FriendRequest> src, bool incomingList) => src.Select(r => new FriendRequestDto
            {
                Id = r.Id,
                FromUserId = r.FromUserId,
                ToUserId = r.ToUserId,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                RespondedAt = r.RespondedAt,
                CommonFriendsCount = commonCounts.TryGetValue(incomingList ? r.FromUserId : r.ToUserId, out var c) ? c : 0
            }).ToList();

            return (Map(incoming, true), Map(outgoing, false));
        }

        public async Task<(bool ok, string? error)> AcceptAsync(int requestId, string currentUserId)
        {
            var req = await _frRepo.GetByIdAsync(requestId);
            if (req == null || req.ToUserId != currentUserId) return (false, "Solicitud no encontrada");
            if (req.Status != FriendRequestStatus.Pending) return (false, "La solicitud ya fue respondida");
            req.Status = FriendRequestStatus.Accepted;
            req.RespondedAt = DateTime.UtcNow;
            req.UpdatedBy = currentUserId;
            await _frRepo.UpdateAsync(req);

            var existingFs = await _fsRepo.FindIncludingDeletedAsync(req.FromUserId, req.ToUserId);
            if (existingFs != null)
            {
                if (existingFs.IsDeleted)
                {
                    existingFs.IsDeleted = false;
                    existingFs.UpdatedBy = currentUserId;
                    await _fsRepo.UpdateAsync(existingFs);
                }
            }
            else
            {
                await _fsRepo.AddAsync(new Friendship { UserId = req.FromUserId, FriendId = req.ToUserId, CreatedBy = currentUserId });
            }
            return (true, null);
        }

        public async Task<(bool ok, string? error)> RejectAsync(int requestId, string currentUserId)
        {
            var req = await _frRepo.GetByIdAsync(requestId);
            if (req == null || req.ToUserId != currentUserId) return (false, "Solicitud no encontrada");
            if (req.Status != FriendRequestStatus.Pending) return (false, "La solicitud ya fue respondida");
            req.Status = FriendRequestStatus.Rejected;
            req.RespondedAt = DateTime.UtcNow;
            req.UpdatedBy = currentUserId;
            await _frRepo.UpdateAsync(req);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> DeleteAsync(int requestId, string currentUserId)
        {
            var req = await _frRepo.GetByIdAsync(requestId);
            if (req == null || req.FromUserId != currentUserId) return (false, "Solicitud no encontrada");
            if (req.Status != FriendRequestStatus.Pending) return (false, "Solo puede eliminar solicitudes pendientes");
            req.UpdatedBy = currentUserId;
            await _frRepo.DeleteAsync(req);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> CreateAsync(string currentUserId, string toUserId)
        {
            if (currentUserId == toUserId) return (false, "No puede enviarse solicitud a sí mismo");
            if (await _fsRepo.AreFriendsAsync(currentUserId, toUserId)) return (false, "Ya son amigos");

            // No permitir crear si el otro usuario ya me envió una solicitud pendiente
            var inverse = await _frRepo.GetAsync(toUserId, currentUserId);
            if (inverse != null && inverse.Status == FriendRequestStatus.Pending)
            {
                return (false, "Existe una solicitud pendiente entre ambos usuarios");
            }

            var existingAny = await _frRepo.GetIncludingDeletedAsync(currentUserId, toUserId);
            if (existingAny != null)
            {
                if (existingAny.IsDeleted)
                {
                    existingAny.IsDeleted = false;
                    existingAny.Status = FriendRequestStatus.Pending;
                    existingAny.RespondedAt = null;
                    existingAny.UpdatedBy = currentUserId;
                    await _frRepo.UpdateAsync(existingAny);
                    return (true, null);
                }
                if (existingAny.Status == FriendRequestStatus.Pending)
                {
                    return (false, "Existe una solicitud pendiente entre ambos usuarios");
                }
                if (existingAny.Status == FriendRequestStatus.Rejected)
                {
                    existingAny.Status = FriendRequestStatus.Pending;
                    existingAny.RespondedAt = null;
                    existingAny.UpdatedBy = currentUserId;
                    await _frRepo.UpdateAsync(existingAny);
                    return (true, null);
                }
                if (existingAny.Status == FriendRequestStatus.Accepted)
                {
                    // Si fueron amigos pero ya no (no hay friendship activa), permitir reenviar reutilizando la fila
                    var stillFriends = await _fsRepo.AreFriendsAsync(currentUserId, toUserId);
                    if (!stillFriends)
                    {
                        existingAny.Status = FriendRequestStatus.Pending;
                        existingAny.RespondedAt = null;
                        existingAny.UpdatedBy = currentUserId;
                        await _frRepo.UpdateAsync(existingAny);
                        return (true, null);
                    }
                    return (false, "Ya son amigos");
                }
            }

            await _frRepo.AddAsync(new FriendRequest { FromUserId = currentUserId, ToUserId = toUserId, Status = FriendRequestStatus.Pending, CreatedBy = currentUserId });
            return (true, null);
        }

        public async Task<IReadOnlyList<EligibleUserDto>> GetEligibleUsersAsync(string currentUserId, string? search)
        {
            var candidates = await _identityReadService.SearchActiveUsersAsync(search ?? string.Empty);
            var result = new List<EligibleUserDto>(candidates.Count);
            var currentFriends = (await _fsRepo.GetFriendsAsync(currentUserId))
                .Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId)
                .ToHashSet();

            foreach (var c in candidates)
            {
                if (c.UserId == currentUserId) continue;
                if (currentFriends.Contains(c.UserId)) continue;
                var existing = await _frRepo.GetAsync(currentUserId, c.UserId);
                var inv = await _frRepo.GetAsync(c.UserId, currentUserId);
                if ((existing != null && existing.Status == FriendRequestStatus.Pending) ||
                    (inv != null && inv.Status == FriendRequestStatus.Pending))
                {
                    continue;
                }
                var theirFriends = (await _fsRepo.GetFriendsAsync(c.UserId))
                    .Select(f => f.UserId == c.UserId ? f.FriendId : f.UserId)
                    .ToHashSet();
                var common = currentFriends.Intersect(theirFriends).Count();

                result.Add(new EligibleUserDto
                {
                    UserId = c.UserId,
                    UserName = c.UserName,
                    FullName = c.FullName,
                    ProfilePhotoUrl = c.ProfilePhotoUrl,
                    CommonFriendsCount = common
                });
            }
            return result;
        }

        private async Task<Dictionary<string, int>> GetCommonFriendsCountsAsync(string currentUserId, IEnumerable<string> otherUserIds)
        {
            var dict = new Dictionary<string, int>();
            var currentFriends = (await _fsRepo.GetFriendsAsync(currentUserId)).Select(f => f.UserId == currentUserId ? f.FriendId : f.UserId).ToHashSet();
            foreach (var other in otherUserIds.Distinct())
            {
                var theirFriends = (await _fsRepo.GetFriendsAsync(other)).Select(f => f.UserId == other ? f.FriendId : f.UserId).ToHashSet();
                dict[other] = currentFriends.Intersect(theirFriends).Count();
            }
            return dict;
        }
    }
}
