using System.Diagnostics;
using LinkUp.Core.Application.Dtos.Social;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Application.Mappings.DtosAndViewModels;
using LinkUp.Core.Application.Services;
using LinkUp.Core.Application.ViewModels.Home;
using LinkUp.Core.Application.ViewModels.Posts;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Social;
using LinkUp.Infrastructure.Identity.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using LinkUp.Helpers;

namespace LinkUp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IReactionService _reactionService;
        private readonly IIdentityReadService _identityReadService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IFileManager _fileManager;

        public HomeController(ILogger<HomeController> logger, IPostService postService, ICommentService commentService, IReactionService reactionService, IIdentityReadService identityReadService, UserManager<AppUser> userManager, IMapper mapper, IFileManager fileManager)
        {
            _logger = logger;
            _postService = postService;
            _commentService = commentService;
            _reactionService = reactionService;
            _identityReadService = identityReadService;
            _userManager = userManager;
            _mapper = mapper;
            _fileManager = fileManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            var posts = await _postService.GetMyPostsAsync(user.Id);

            var ids = posts.Select(p => p.UserId)
                .Concat(posts.SelectMany(p => p.Comments).Select(c => c.UserId))
                .Concat(posts.SelectMany(p => p.Comments).SelectMany(c => c.Replies).Select(r => r.UserId))
                .ToHashSet();
            var profiles = await _identityReadService.GetByIdsAsync(ids);
            var dict = profiles.ToDictionary(p => p.UserId, p => p);

            var vm = new HomeViewModel
            {
                Create = new(),
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
            ViewBag.UserId = user.Id;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(CreatePostViewModel vm)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");

            string? savedImage = null;
            if (vm.MediaKind == "image" && vm.Image != null && vm.Image.Length > 0)
            {
                savedImage = await _fileManager.SaveAsync(vm.Image, "uploads", new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }, 5 * 1024 * 1024);
                if (savedImage is null)
                {
                    TempData["Error"] = "Imagen inválida o demasiado grande (máx 5MB)";
                    return RedirectToAction("Index");
                }
            }

            var dto = new CreatePostDto
            {
                Content = vm.Content,
                MediaKind = vm.MediaKind,
                YouTubeUrl = vm.YouTubeUrl,
                SavedImagePath = savedImage
            };

            var (ok, error) = await _postService.CreateAsync(user.Id, dto);
            if (!ok) TempData["Error"] = error;
            return RedirectToAction("Index");
        }
    }
}
