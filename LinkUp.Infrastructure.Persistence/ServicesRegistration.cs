using LinkUp.Core.Domain.Interfaces.Persistence;
using LinkUp.Infrastructure.Persistence.Contexts;
using LinkUp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IBattleshipGameRepository, BattleshipGameRepository>();

            return services;
        }
    }
}
