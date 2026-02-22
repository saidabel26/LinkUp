using LinkUp.Core.Application.ViewModels.Profile;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LinkUp.Helpers;

namespace LinkUp.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileManager _fileManager;

        public ProfileController(UserManager<AppUser> userManager, IFileManager fileManager)
        {
            _userManager = userManager;
            _fileManager = fileManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var vm = new EditProfileViewModel
            {
                FirstName = me.FirstName ?? string.Empty,
                LastName = me.LastName ?? string.Empty,
                Phone = me.PhoneNumber ?? string.Empty
            };
            ViewBag.Photo = me.ProfilePhotoUrl;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EditProfileViewModel vm)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            if (!ModelState.IsValid)
            {
                ViewBag.Photo = me.ProfilePhotoUrl;
                return View(vm);
            }

            me.FirstName = vm.FirstName;
            me.LastName = vm.LastName;
            me.PhoneNumber = vm.Phone;

            if (vm.ProfilePhoto is not null && vm.ProfilePhoto.Length > 0)
            {
                var saved = await _fileManager.SaveAsync(vm.ProfilePhoto, "uploads", new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }, 2 * 1024 * 1024);
                if (saved is null)
                {
                    ModelState.AddModelError(string.Empty, "Archivo de imagen inválido o demasiado grande (máx 2MB)");
                    ViewBag.Photo = me.ProfilePhotoUrl;
                    return View(vm);
                }
                me.ProfilePhotoUrl = saved;
            }

            var update = await _userManager.UpdateAsync(me);
            if (!update.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join("; ", update.Errors.Select(e => e.Description)));
                ViewBag.Photo = me.ProfilePhotoUrl;
                return View(vm);
            }

            if (!string.IsNullOrWhiteSpace(vm.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(me);
                var reset = await _userManager.ResetPasswordAsync(me, token, vm.Password);
                if (!reset.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, string.Join("; ", reset.Errors.Select(e => e.Description)));
                    ViewBag.Photo = me.ProfilePhotoUrl;
                    return View(vm);
                }
            }

            TempData["ProfileSuccess"] = "Perfil actualizado";
            return RedirectToAction(nameof(Index));
        }
    }
}
