using LinkUp.Core.Application.Dtos.Battleship;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Controllers
{
    [Authorize]
    public class BattleshipController : Controller
    {
        private readonly IBattleshipService _service;
        private readonly IFriendsFeedService _friendsFeed;
        private readonly IIdentityReadService _identityRead;
        private readonly UserManager<AppUser> _userManager;

        public BattleshipController(IBattleshipService service, IFriendsFeedService friendsFeed, IIdentityReadService identityRead, UserManager<AppUser> userManager)
        {
            _service = service;
            _friendsFeed = friendsFeed;
            _identityRead = identityRead;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var active = await _service.GetActiveGamesAsync(me.Id);
            var history = await _service.GetHistoryAsync(me.Id);

            var activeFriendIds = active.Select(g => g.CreatorUserId == me.Id ? g.OpponentUserId : g.CreatorUserId).ToHashSet();
            var historyFriendIds = history.Select(g => g.OpponentUserId).ToHashSet();
            var allIds = activeFriendIds.Union(historyFriendIds).ToHashSet();
            var profiles = await _identityRead.GetByIdsAsync(allIds);
            var profDict = profiles.ToDictionary(p => p.UserId, p => p);

            var activeFriendByGame = active.ToDictionary(g => g.Id, g =>
            {
                var fid = g.CreatorUserId == me.Id ? g.OpponentUserId : g.CreatorUserId;
                if (!profDict.TryGetValue(fid, out var p)) p = null;
                var name = p?.FullName ?? p?.UserName ?? fid;
                return (userId: fid, display: name);
            });
            var historyFriendByGame = history.ToDictionary(g => g.Id, g =>
            {
                var fid = g.OpponentUserId;
                if (!profDict.TryGetValue(fid, out var p)) p = null;
                var name = p?.FullName ?? p?.UserName ?? fid;
                return (userId: fid, display: name);
            });

            ViewBag.Active = active;
            ViewBag.History = history;
            ViewBag.ActiveFriendByGame = activeFriendByGame;
            ViewBag.HistoryFriendByGame = historyFriendByGame;
            ViewBag.HistorySummary = new { total = history.Count, won = history.Count(h => h.Won), lost = history.Count(h => !h.Won) };
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> New(string? q)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var friends = await _friendsFeed.GetFriendsAsync(me.Id);
            if (!string.IsNullOrWhiteSpace(q))
                friends = friends.Where(f => f.userName.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();
            // Excluir amigos con partidas activas ya existentes
            var active = await _service.GetActiveGamesAsync(me.Id);
            var blocked = active.Select(g => g.CreatorUserId == me.Id ? g.OpponentUserId : g.CreatorUserId).ToHashSet();
            friends = friends.Where(f => !blocked.Contains(f.userId)).ToList();
            ViewBag.Search = q ?? string.Empty;
            ViewBag.Friends = friends;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string opponentUserId)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            if (string.IsNullOrWhiteSpace(opponentUserId))
            {
                TempData["Error"] = "Debe seleccionar un amigo";
                return RedirectToAction(nameof(New));
            }
            var (ok, error, gameId) = await _service.StartGameAsync(me.Id, opponentUserId);
            if (!ok)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Enter), new { id = gameId });
        }

        [HttpGet]
        public async Task<IActionResult> Enter(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (okSum, errSum, sum) = await _service.GetSummaryAsync(id, me.Id);
            if (!okSum || sum == null)
            {
                TempData["Error"] = errSum;
                return RedirectToAction(nameof(Index));
            }
            var (okBoard, errBoard, board) = await _service.GetMyBoardAsync(id, me.Id);
            if (!okBoard || board == null)
            {
                TempData["Error"] = errBoard;
                return RedirectToAction(nameof(Index));
            }
            if (sum.Status == LinkUp.Core.Domain.Common.Enums.GameStatus.Setup)
            {
                if (board.MyPlacements.Count < 5)
                    return RedirectToAction(nameof(Setup), new { id });
                else
                    return RedirectToAction(nameof(Waiting), new { id });
            }
            else if (sum.Status == LinkUp.Core.Domain.Common.Enums.GameStatus.Active)
            {
                return RedirectToAction(nameof(Attack), new { id });
            }
            else
            {
                return RedirectToAction(nameof(Result), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Setup(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (ok, err, board) = await _service.GetMyBoardAsync(id, me.Id);
            if (!ok || board == null) { TempData["Error"] = err; return RedirectToAction(nameof(Index)); }
            var counts = board.MyPlacements.GroupBy(p => p.Length).ToDictionary(g => g.Key, g => g.Count());
            ViewBag.GameId = id;
            ViewBag.Remaining = new[] { 2, 3, 3, 4, 5 }
                .GroupBy(x => x)
                .Select(g => new { Length = g.Key, Remaining = g.Count() - (counts.TryGetValue(g.Key, out var used) ? used : 0) })
                .Where(x => x.Remaining > 0)
                .ToList();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ChooseCell(int id, int length)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (ok, err, board) = await _service.GetMyBoardAsync(id, me.Id);
            if (!ok || board == null) { TempData["Error"] = err; return RedirectToAction(nameof(Index)); }
            ViewBag.GameId = id;
            ViewBag.Length = length;
            ViewBag.Board = board;
            return View();
        }

        [HttpGet]
        public IActionResult ChooseDirection(int id, int length, int row, int col)
        {
            ViewBag.GameId = id;
            ViewBag.Length = length;
            ViewBag.Row = row;
            ViewBag.Col = col;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceShip(PlaceShipRequestDto dto)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (ok, err) = await _service.PlaceShipAsync(me.Id, dto);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(ChooseDirection), new { id = dto.GameId, length = dto.Length, row = dto.StartRow, col = dto.StartCol });
            }
            return RedirectToAction(nameof(Enter), new { id = dto.GameId });
        }

        [HttpGet]
        public async Task<IActionResult> Waiting(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (ok, err, board) = await _service.GetMyBoardAsync(id, me.Id);
            if (!ok || board == null) { TempData["Error"] = err; return RedirectToAction(nameof(Index)); }
            ViewBag.GameId = id;
            ViewBag.Board = board;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Attack(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (okSum, errSum, sum) = await _service.GetSummaryAsync(id, me.Id);
            if (!okSum || sum == null) { TempData["Error"] = errSum; return RedirectToAction(nameof(Index)); }
            if (sum.Status == LinkUp.Core.Domain.Common.Enums.GameStatus.Finished)
            {
                return RedirectToAction(nameof(Index));
            }
            if (sum.Status == LinkUp.Core.Domain.Common.Enums.GameStatus.Setup)
            {
                var (okBoard, errBoard, b) = await _service.GetMyBoardAsync(id, me.Id);
                if (!okBoard || b == null) { TempData["Error"] = errBoard; return RedirectToAction(nameof(Index)); }
                if (b.MyPlacements.Count < 5) return RedirectToAction(nameof(Setup), new { id });
                else return RedirectToAction(nameof(Waiting), new { id });
            }
            var (ok, err, board) = await _service.GetMyBoardAsync(id, me.Id);
            if (!ok || board == null) { TempData["Error"] = err; return RedirectToAction(nameof(Index)); }
            ViewBag.GameId = id;
            ViewBag.Board = board;
            ViewBag.IsMyTurn = sum.CurrentTurnUserId == me.Id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoAttack(AttackRequestDto dto)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (ok, err) = await _service.AttackAsync(me.Id, dto);
            if (!ok)
            {
                TempData["Error"] = err;
                return RedirectToAction(nameof(Attack), new { id = dto.GameId });
            }
            var (okSum, errSum, sum) = await _service.GetSummaryAsync(dto.GameId, me.Id);
            if (okSum && sum is not null && sum.Status == LinkUp.Core.Domain.Common.Enums.GameStatus.Finished)
            {
                if (sum.WinnerUserId == me.Id)
                {
                    TempData["BattleSuccess"] = "¡Ganaste! Felicidades, eres el ganador.";
                }
                else
                {
                    TempData["BattleInfo"] = "La partida ha finalizado.";
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Attack), new { id = dto.GameId });
        }

        [HttpGet]
        public async Task<IActionResult> MyBoard(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var (ok, err, board) = await _service.GetMyBoardAsync(id, me.Id);
            if (!ok || board == null) { TempData["Error"] = err; return RedirectToAction(nameof(Index)); }
            ViewBag.Board = board;
            ViewBag.GameId = id;
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmResign(int id)
        {
            ViewBag.GameId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resign(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            await _service.ResignAsync(id, me.Id);
            TempData["BattleInfo"] = "Te has rendido. La partida ha sido finalizada.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Result(int id, bool showOpponent = false)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return RedirectToAction("Login", "Account");
            var method = showOpponent ? _service.GetOpponentAttackBoardAsync(id, me.Id) : _service.GetMyBoardAsync(id, me.Id);
            var (ok, error, board) = await method;
            if (!ok)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Board = board;
            ViewBag.ShowOpponent = showOpponent;
            ViewBag.GameId = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Status(int id)
        {
            var me = await _userManager.GetUserAsync(User);
            if (me == null) return Unauthorized();
            var (ok, err, sum) = await _service.GetSummaryAsync(id, me.Id);
            if (!ok || sum == null) return Json(new { ok = false, error = err });
            return Json(new { ok = true, status = sum.Status.ToString(), winnerUserId = sum.WinnerUserId });
        }
    }
}
