using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.Posts;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<AppUser> _userManager;

        public CommentsController(ICommentService commentService, UserManager<AppUser> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CommentCreateViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid)
            {
                return RedirectToRefererOrHome();
            }
            if (vm.ParentCommentId.HasValue)
                await _commentService.AddReplyAsync(vm.ParentCommentId.Value, user.Id, vm.Content);
            else
                await _commentService.AddCommentAsync(vm.PostId, user.Id, vm.Content);
            return RedirectToRefererOrHome();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string content)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            await _commentService.EditCommentAsync(id, user.Id, content);
            return RedirectToRefererOrHome();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            await _commentService.DeleteCommentAsync(id, user.Id);
            return RedirectToRefererOrHome();
        }

        private IActionResult RedirectToRefererOrHome()
        {
            var referer = Request.Headers["Referer"].ToString();
            if (Uri.IsWellFormedUriString(referer, UriKind.Absolute)) return Redirect(referer);
            return RedirectToAction("Index", "Home");
        }
    }
}
