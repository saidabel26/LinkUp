using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    [Authorize]
    public class ReactionsController : Controller
    {
        private readonly IReactionService _reactionService;
        private readonly UserManager<AppUser> _userManager;

        public ReactionsController(IReactionService reactionService, UserManager<AppUser> userManager)
        {
            _reactionService = reactionService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> React(int postId, ReactionType type)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            await _reactionService.ReactAsync(postId, user.Id, type);
            var referer = Request.Headers["Referer"].ToString();
            if (Uri.IsWellFormedUriString(referer, UriKind.Absolute)) return Redirect(referer);
            return RedirectToAction("Index", "Home");
        }
    }
}
