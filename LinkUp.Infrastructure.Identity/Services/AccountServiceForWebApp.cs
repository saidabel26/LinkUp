using LinkUp.Core.Application.Interfaces;
using LinkUp.Infrastructure.Identity.Entities;
using LinkUp.Infrastructure.Identity; // para tipos record
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LinkUp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApp : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AccountServiceForWebApp(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<AuthResult> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
                return new(false, "Usuario o contraseña incorrectos", false);

            if (!user.EmailConfirmed)
                return new(false, null, true);

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, isPersistent: true, lockoutOnFailure: false);
            if (!result.Succeeded)
                return new(false, "Usuario o contraseña incorrectos", false);

            return new(true, null, false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<RegisterResult> RegisterAsync(RegisterRequest request, string originBaseUrl)
        {
            if (await _userManager.FindByNameAsync(request.UserName) is not null)
            {
                return new(false, new[] { "El nombre de usuario ya existe" }, null);
            }

            var configuredEmailFrom = _configuration.GetSection("MailSettings").GetValue<string>("EmailFrom") ?? string.Empty;
            var emailInput = request.Email?.Trim();
            if (string.IsNullOrWhiteSpace(emailInput))
            {
                return new(false, new[] { "El correo es requerido" }, null);
            }
            var isEmailFrom = string.Equals(emailInput, configuredEmailFrom.Trim(), StringComparison.OrdinalIgnoreCase);
            var normalized = emailInput.ToUpperInvariant();

            // Enforzar unicidad para correos distintos a EmailFrom
            if (!isEmailFrom)
            {
                var exists = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalized);
                if (exists)
                {
                    return new(false, new[] { "Ya existe una cuenta con ese correo" }, null);
                }
            }

            // Para EmailFrom, permitir duplicados; solo detectar si ya existía para informar
            bool hadExistingSameEmail = false;
            if (isEmailFrom)
            {
                hadExistingSameEmail = await _userManager.Users.AnyAsync(u => u.NormalizedEmail == normalized);
            }

            var user = new AppUser
            {
                UserName = request.UserName,
                Email = emailInput,
                PhoneNumber = request.Phone,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ProfilePhotoUrl = request.ProfilePhotoUrl
            };
            var create = await _userManager.CreateAsync(user, request.Password);
            if (!create.Succeeded)
            {
                return new(false, create.Errors.Select(e => e.Description), null);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = $"{originBaseUrl}/Account/ConfirmEmail?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendAsync(user.Email!, "Confirma tu cuenta", $"<p>Hola {request.FirstName}, confirma tu cuenta haciendo clic <a href='{url}'>aquí</a>.</p>");

            var info = (isEmailFrom && hadExistingSameEmail)
                ? "Correo duplicado con remitente SMTP para validar funcionalidades del Proyecto"
                : null;
            return new(true, Array.Empty<string>(), info);
        }

        public async Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordRequest request, string originBaseUrl)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user is null)
                return new(false, "Usuario no encontrado");

            // Desactivar temporalmente la cuenta
            user.EmailConfirmed = false;
            await _userManager.UpdateAsync(user);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = $"{originBaseUrl}/Account/ResetPassword?userId={Uri.EscapeDataString(user.Id)}&token={Uri.EscapeDataString(token)}";
            await _emailService.SendAsync(user.Email!, "Restablecer contraseña", $"<p>Para restablecer su contraseña haga clic <a href='{url}'>aquí</a>.</p>");

            return new(true, null);
        }

        public async Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return new(false, new[] { "Usuario no encontrado" });

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
                return new(false, result.Errors.Select(e => e.Description));

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return new(true, Array.Empty<string>());
        }

        public async Task<ConfirmEmailResult> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return new(false, "Usuario no encontrado");

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
                return new(false, string.Join("; ", result.Errors.Select(e => e.Description)));

            return new(true, null);
        }
    }
}
