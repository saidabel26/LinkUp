using LinkUp.Infrastructure.Identity.Contexts;
using LinkUp.Infrastructure.Identity.Entities;
using LinkUp.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Infrastructure.Identity
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<IdentityContext>(options => options.UseSqlServer(connectionString));

            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                // Se controla la unicidad de email en el servicio (permitiendo duplicado solo para EmailFrom)
                options.User.RequireUniqueEmail = false;
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<IAccountService, AccountServiceForWebApp>();
            services.AddScoped<LinkUp.Core.Application.Interfaces.IIdentityReadService, IdentityReadService>();

            return services;
        }
    }

    public interface IAccountService
    {
        Task<RegisterResult> RegisterAsync(RegisterRequest request, string originBaseUrl);
        Task<AuthResult> LoginAsync(LoginRequest request);
        Task LogoutAsync();
        Task<ForgotPasswordResult> ForgotPasswordAsync(ForgotPasswordRequest request, string originBaseUrl);
        Task<ResetPasswordResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ConfirmEmailResult> ConfirmEmailAsync(string userId, string token);
    }

    public record RegisterRequest(string FirstName, string LastName, string Phone, string Email, string UserName, string Password, string? ProfilePhotoUrl);
    public record LoginRequest(string UserName, string Password);
    public record ForgotPasswordRequest(string UserName);
    public record ResetPasswordRequest(string UserId, string Token, string NewPassword);

    public record RegisterResult(bool Succeeded, IEnumerable<string> Errors, string? InfoMessage);
    public record AuthResult(bool Succeeded, string? Error, bool IsNotConfirmed);
    public record ForgotPasswordResult(bool Succeeded, string? Error);
    public record ResetPasswordResult(bool Succeeded, IEnumerable<string> Errors);
    public record ConfirmEmailResult(bool Succeeded, string? Error);
}
