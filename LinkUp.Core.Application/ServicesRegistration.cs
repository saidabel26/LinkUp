using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkUp.Core.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using LinkUp.Core.Application.Services;

namespace LinkUp.Core.Application
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IReactionService, ReactionService>();
            services.AddScoped<IFriendsService, FriendsService>();
            services.AddScoped<IFriendRequestsService, FriendRequestsService>();
            services.AddScoped<IFriendsFeedService, FriendsFeedService>();
            services.AddScoped<IBattleshipService, BattleshipService>();
            return services;
        }
    }
}
