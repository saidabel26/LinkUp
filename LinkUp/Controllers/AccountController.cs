using LinkUp.Core.Application.ViewModels.Account;
using LinkUp.Infrastructure.Identity;
using LinkUp.Infrastructure.Identity.Services;
using LinkUp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IFileManager _fileManager;

        public AccountController(IAccountService accountService, IFileManager fileManager)
        {
            _accountService = accountService;
            _fileManager = fileManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            // Mensaje cuando intenta acceder a una sección protegida
            if (Request.Query.TryGetValue("msg", out var msg) && msg == "auth")
            {
                TempData["AuthRequired"] = "Debe iniciar sesión para acceder a esta sección";
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var result = await _accountService.LoginAsync(new LoginRequest(vm.UserName, vm.Password));
            if (!result.Succeeded)
            {
                if (result.IsNotConfirmed)
                {
                    vm.IsInactive = true;
                    vm.ErrorMessage = "Cuenta inactiva. Revise su correo para activarla.";
                }
                else
                {
                    vm.ErrorMessage = result.Error ?? "Datos inválidos";
                }
                return View(vm);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            string? photoUrl = null;
            if (vm.ProfilePhoto is not null && vm.ProfilePhoto.Length > 0)
            {
                photoUrl = await _fileManager.SaveAsync(vm.ProfilePhoto, "uploads", new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }, 2 * 1024 * 1024);
                if (photoUrl is null)
                {
                    ModelState.AddModelError(string.Empty, "Foto inválida o demasiado grande (máx 2MB)");
                    return View(vm);
                }
            }

            var result = await _accountService.RegisterAsync(new RegisterRequest(vm.FirstName, vm.LastName, vm.Phone, vm.Email, vm.UserName, vm.Password, photoUrl),
                $"{Request.Scheme}://{Request.Host}");
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e);
                return View(vm);
            }
            vm.Success = "Registro exitoso. Revise su correo para activar la cuenta.";
            ModelState.Clear();
            return View(new RegisterViewModel { Success = vm.Success });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var result = await _accountService.ForgotPasswordAsync(new ForgotPasswordRequest(vm.UserName), $"{Request.Scheme}://{Request.Host}");
            if (!result.Succeeded)
            {
                vm.Error = result.Error;
                return View(vm);
            }
            vm.Message = "Se envió un correo con el enlace para restablecer la contraseña.";
            ModelState.Clear();
            return View(new ForgotPasswordViewModel { Message = vm.Message });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token)
        {
            return View(new ResetPasswordViewModel { UserId = userId, Token = token });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var result = await _accountService.ResetPasswordAsync(new ResetPasswordRequest(vm.UserId, vm.Token, vm.Password));
            if (!result.Succeeded)
            {
                vm.Error = string.Join("; ", result.Errors);
                return View(vm);
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _accountService.ConfirmEmailAsync(userId, token);
            TempData["ConfirmMessage"] = result.Succeeded ? "Cuenta activada correctamente." : result.Error ?? "No se pudo confirmar la cuenta.";
            return RedirectToAction("Login");
        }
    }
}
