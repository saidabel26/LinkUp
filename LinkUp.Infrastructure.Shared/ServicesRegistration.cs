using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Settings;
using LinkUp.Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Infrastructure.Shared
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.AddTransient<IEmailService, EmailService>();
            return services;
        }
    }
}
