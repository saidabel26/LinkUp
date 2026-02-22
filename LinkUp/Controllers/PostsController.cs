using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.Posts;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;
        private readonly UserManager<AppUser> _userManager;

        public PostsController(IPostService postService, UserManager<AppUser> userManager)
        {
            _postService = postService;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPostViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var dto = new EditPostDto { Id = vm.Id, Content = vm.Content };
            var result = await _postService.UpdateAsync(user.Id, dto);
            TempData[result.ok ? "Success" : "Error"] = result.ok ? "Publicación actualizada" : result.error;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var result = await _postService.DeleteAsync(user.Id, id);
            TempData[result.ok ? "Success" : "Error"] = result.ok ? "Publicación eliminada" : result.error;
            return RedirectToAction("Index", "Home");
        }
    }
}
