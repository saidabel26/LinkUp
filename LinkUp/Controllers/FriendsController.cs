using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.ViewModels.Home;
using LinkUp.Core.Application.ViewModels.Posts;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    [Authorize]
    public class FriendsController : Controller
    {
        private readonly IFriendsService _friendsService;
        private readonly IFriendsFeedService _feedService;
        private readonly IIdentityReadService _identityReadService;
        private readonly UserManager<AppUser> _userManager;

        public FriendsController(IFriendsService friendsService, IFriendsFeedService feedService, IIdentityReadService identityReadService, UserManager<AppUser> userManager)
        {
            _friendsService = friendsService;
            _feedService = feedService;
            _identityReadService = identityReadService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(base.User);
            if (user == null) return RedirectToAction("Login", "Account");
            var posts = await _feedService.GetFriendsPostsAsync(user.Id);
            var friendBasics = await _feedService.GetFriendsAsync(user.Id);
            var ids = posts.Select(p => p.UserId)
                .Concat(posts.SelectMany(p => p.Comments).Select(c => c.UserId))
                .Concat(posts.SelectMany(p => p.Comments).SelectMany(c => c.Replies).Select(r => r.UserId))
                .Concat(friendBasics.Select(f => f.userId))
                .ToHashSet();
            var profiles = await _identityReadService.GetByIdsAsync(ids);
            var dict = profiles.ToDictionary(p => p.UserId, p => p);
            var vm = new HomeViewModel
            {
                Create = new CreatePostViewModel(),
                Posts = posts.Select(p => new PostItemViewModel
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId,
                    AuthorUserName = dict.TryGetValue(p.UserId, out var pu) ? (pu.FullName ?? pu.UserName) : p.UserId,
                    AuthorProfilePhotoUrl = dict.TryGetValue(p.UserId, out var pu2) ? pu2.ProfilePhotoUrl : null,
                    MediaType = p.MediaType,
                    ImageUrl = p.ImageUrl,
                    YouTubeUrl = p.YouTubeUrl,
                    MyReaction = p.MyReaction,
                    Comments = p.Comments
                        .Select(c => new CommentItemViewModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            CreatedAt = c.CreatedAt,
                            UserId = c.UserId,
                            AuthorUserName = dict.TryGetValue(c.UserId, out var cu) ? (cu.FullName ?? cu.UserName) : c.UserId,
                            AuthorProfilePhotoUrl = dict.TryGetValue(c.UserId, out var cu2) ? cu2.ProfilePhotoUrl : null,
                            Replies = c.Replies.Select(r => new CommentItemViewModel
                            {
                                Id = r.Id,
                                Content = r.Content,
                                CreatedAt = r.CreatedAt,
                                UserId = r.UserId,
                                AuthorUserName = dict.TryGetValue(r.UserId, out var ru) ? (ru.FullName ?? ru.UserName) : r.UserId,
                                AuthorProfilePhotoUrl = dict.TryGetValue(r.UserId, out var ru2) ? ru2.ProfilePhotoUrl : null,
                            }).ToList()
                        }).ToList()
                }).ToList()
            };
            ViewBag.Friends = friendBasics.Select(f => new { f.userId, name = dict.TryGetValue(f.userId, out var p) ? (p.FullName ?? p.UserName) : f.userId, photo = dict.TryGetValue(f.userId, out var p2) ? p2.ProfilePhotoUrl : null }).ToList();
            return View(vm);
        }

        [HttpGet]
        [ActionName("User")]
        public async Task<IActionResult> UserPosts(string id)
        {
            var me = await _userManager.GetUserAsync(base.User);
            if (me == null) return RedirectToAction("Login", "Account");
            var posts = (await _feedService.GetFriendsPostsAsync(me.Id)).Where(p => p.UserId == id).ToList();
            var ids = posts.Select(p => p.UserId)
                .Concat(posts.SelectMany(p => p.Comments).Select(c => c.UserId))
                .Concat(posts.SelectMany(p => p.Comments).SelectMany(c => c.Replies).Select(r => r.UserId))
                .ToHashSet();
            var profiles = await _identityReadService.GetByIdsAsync(ids);
            var dict = profiles.ToDictionary(p => p.UserId, p => p);
            var vm = new HomeViewModel
            {
                Create = new CreatePostViewModel(),
                Posts = posts.Select(p => new PostItemViewModel
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    UserId = p.UserId,
                    AuthorUserName = dict.TryGetValue(p.UserId, out var pu) ? (pu.FullName ?? pu.UserName) : p.UserId,
                    AuthorProfilePhotoUrl = dict.TryGetValue(p.UserId, out var pu2) ? pu2.ProfilePhotoUrl : null,
                    MediaType = p.MediaType,
                    ImageUrl = p.ImageUrl,
                    YouTubeUrl = p.YouTubeUrl,
                    MyReaction = p.MyReaction,
                    Comments = p.Comments
                        .Select(c => new CommentItemViewModel
                        {
                            Id = c.Id,
                            Content = c.Content,
                            CreatedAt = c.CreatedAt,
                            UserId = c.UserId,
                            AuthorUserName = dict.TryGetValue(c.UserId, out var cu) ? (cu.FullName ?? cu.UserName) : c.UserId,
                            AuthorProfilePhotoUrl = dict.TryGetValue(c.UserId, out var cu2) ? cu2.ProfilePhotoUrl : null,
                            Replies = c.Replies.Select(r => new CommentItemViewModel
                            {
                                Id = r.Id,
                                Content = r.Content,
                                CreatedAt = r.CreatedAt,
                                UserId = r.UserId,
                                AuthorUserName = dict.TryGetValue(r.UserId, out var ru) ? (ru.FullName ?? ru.UserName) : r.UserId,
                                AuthorProfilePhotoUrl = dict.TryGetValue(r.UserId, out var ru2) ? ru2.ProfilePhotoUrl : null,
                            }).ToList()
                        }).ToList()
                }).ToList()
            };
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmRemove(string id)
        {
            var me = await _userManager.GetUserAsync(base.User);
            if (me == null) return RedirectToAction("Login", "Account");
            var prof = (await _identityReadService.GetByIdsAsync(new[] { id })).FirstOrDefault();
            ViewBag.UserDisplay = prof?.UserName ?? id;
            ViewBag.UserId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(string id)
        {
            var me = await _userManager.GetUserAsync(base.User);
            if (me == null) return RedirectToAction("Login", "Account");
            await _friendsService.RemoveFriendAsync(me.Id, id);
            return RedirectToAction(nameof(Index));
        }
    }
}
