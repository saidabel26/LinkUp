using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    [Authorize]
    public class FriendRequestsController : Controller
    {
        private readonly IFriendRequestsService _service;
        private readonly UserManager<AppUser> _userManager;
        private readonly IIdentityReadService _identityReadService;

        public FriendRequestsController(IFriendRequestsService service, UserManager<AppUser> userManager, IIdentityReadService identityReadService)
        {
            _service = service;
            _userManager = userManager;
            _identityReadService = identityReadService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var (incoming, outgoing) = await _service.GetListsAsync(user.Id);
            var ids = incoming.Select(i => i.FromUserId).Concat(outgoing.Select(o => o.ToUserId)).ToHashSet();
            var profiles = (await _identityReadService.GetByIdsAsync(ids)).ToDictionary(x => x.UserId, x => x);
            var vm = new Core.Application.ViewModels.Friends.FriendRequestsIndexViewModel
            {
                Incoming = incoming.ToList(),
                Outgoing = outgoing.ToList(),
                Profiles = profiles
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> New(string? q)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            ViewBag.Search = q ?? string.Empty;
            ViewBag.Users = await _service.GetEligibleUsersAsync(user.Id, q);
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmAccept(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (incoming, _) = await _service.GetListsAsync(me.Id);
            var req = incoming.FirstOrDefault(r => r.Id == id);
            if (req == null) return RedirectToAction(nameof(Index));
            var prof = (await _identityReadService.GetByIdsAsync(new[] { req.FromUserId })).FirstOrDefault();
            return View(new Core.Application.ViewModels.Friends.ConfirmFriendRequestViewModel { Id = id, UserDisplay = prof?.UserName ?? req.FromUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Accept(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            await _service.AcceptAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmReject(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (incoming, _) = await _service.GetListsAsync(me.Id);
            var req = incoming.FirstOrDefault(r => r.Id == id);
            if (req == null) return RedirectToAction(nameof(Index));
            var prof = (await _identityReadService.GetByIdsAsync(new[] { req.FromUserId })).FirstOrDefault();
            return View(new Core.Application.ViewModels.Friends.ConfirmFriendRequestViewModel { Id = id, UserDisplay = prof?.UserName ?? req.FromUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            await _service.RejectAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (_, outgoing) = await _service.GetListsAsync(me.Id);
            var req = outgoing.FirstOrDefault(r => r.Id == id);
            if (req == null) return RedirectToAction(nameof(Index));
            var prof = (await _identityReadService.GetByIdsAsync(new[] { req.ToUserId })).FirstOrDefault();
            return View(new Core.Application.ViewModels.Friends.ConfirmFriendRequestViewModel { Id = id, UserDisplay = prof?.UserName ?? req.ToUserId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            await _service.DeleteAsync(id, user.Id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? toUserId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            if (string.IsNullOrWhiteSpace(toUserId))
            {
                TempData["Error"] = "Debe seleccionar un usuario";
                return RedirectToAction(nameof(New));
            }
            var (ok, error) = await _service.CreateAsync(user.Id, toUserId);
            TempData[ok ? "Success" : "Error"] = ok ? "Solicitud enviada" : error;
            return RedirectToAction(nameof(Index));
        }
    }
}
