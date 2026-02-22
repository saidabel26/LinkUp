using LinkUp.Core.Application.Dtos.Battleship;
using LinkUp.Core.Application.Interfaces;
using LinkUp.Core.Domain.Battleship;
using LinkUp.Core.Domain.Common.Enums;
using LinkUp.Core.Domain.Interfaces.Persistence;

namespace LinkUp.Core.Application.Services
{
    public class BattleshipService : IBattleshipService
    {
        private readonly IBattleshipGameRepository _repo;
        private readonly IFriendshipRepository _friends;

        public BattleshipService(IBattleshipGameRepository repo, IFriendshipRepository friends)
        {
            _repo = repo;
            _friends = friends;
        }

        public async Task<(bool ok, string? error, int? gameId)> StartGameAsync(string currentUserId, string opponentUserId)
        {
            var areFriends = await _friends.AreFriendsAsync(currentUserId, opponentUserId);
            if (!areFriends) return (false, "Solo puedes invitar amigos", null);
            if (await _repo.ExistsActiveBetweenAsync(currentUserId, opponentUserId)) return (false, "Ya existe una partida activa o en configuración", null);

            var game = new BattleshipGame
            {
                CreatorUserId = currentUserId,
                OpponentUserId = opponentUserId,
                Status = GameStatus.Setup,
                CurrentTurnUserId = null,
                StartedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            };
            await _repo.AddAsync(game);
            return (true, null, game.Id);
        }

        public async Task<(bool ok, string? error, GameSummaryDto? game)> GetSummaryAsync(int gameId, string currentUserId)
        {
            var g = await _repo.GetByIdAsync(gameId);
            if (g == null) return (false, "Partida no encontrada", null);
            if (!g.IsUserParticipant(currentUserId)) return (false, "No autorizado", null);
            return (true, null, new GameSummaryDto
            {
                Id = g.Id,
                CreatorUserId = g.CreatorUserId,
                OpponentUserId = g.OpponentUserId,
                Status = g.Status,
                CurrentTurnUserId = g.CurrentTurnUserId,
                StartedAt = g.StartedAt,
                FinishedAt = g.FinishedAt,
                WinnerUserId = g.WinnerUserId,
                ResignedUserId = g.ResignedUserId
            });
        }

        public async Task<IReadOnlyList<GameSummaryDto>> GetActiveGamesAsync(string currentUserId)
        {
            var games = await _repo.GetActiveByUserAsync(currentUserId);
            return games.Select(g => new GameSummaryDto
            {
                Id = g.Id,
                CreatorUserId = g.CreatorUserId,
                OpponentUserId = g.OpponentUserId,
                Status = g.Status,
                CurrentTurnUserId = g.CurrentTurnUserId,
                StartedAt = g.StartedAt,
                FinishedAt = g.FinishedAt,
                WinnerUserId = g.WinnerUserId,
                ResignedUserId = g.ResignedUserId
            }).ToList();
        }

        public async Task<IReadOnlyList<GameHistoryDto>> GetHistoryAsync(string currentUserId)
        {
            var games = await _repo.GetFinishedByUserAsync(currentUserId);
            return games.Select(g => new GameHistoryDto
            {
                Id = g.Id,
                OpponentUserId = g.CreatorUserId == currentUserId ? g.OpponentUserId : g.CreatorUserId,
                StartedAt = g.StartedAt,
                FinishedAt = g.FinishedAt ?? g.StartedAt,
                DurationHours = ((g.FinishedAt ?? DateTime.UtcNow) - g.StartedAt).TotalHours,
                Won = g.WinnerUserId == currentUserId,
                WinnerUserId = g.WinnerUserId ?? string.Empty
            }).ToList();
        }

        public async Task<(bool ok, string? error, BoardDto? board)> GetMyBoardAsync(int gameId, string currentUserId)
        {
            var game = await _repo.GetByIdAsync(gameId);
            if (game == null) return (false, "Partida no encontrada", null);
            if (!game.IsUserParticipant(currentUserId)) return (false, "No autorizado", null);

            var placements = await _repo.GetPlacementsAsync(gameId, currentUserId);
            var attacks = await _repo.GetAttacksAsync(gameId, currentUserId);
            var board = new BoardDto
            {
                Size = 12,
                MyPlacements = placements.Select(p => new PlacementDto { Length = p.Length, StartRow = p.StartRow, StartCol = p.StartCol, Direction = p.Direction }).ToList(),
                MyAttacks = attacks.Select(a => new AttackDto { Row = a.Row, Col = a.Col, IsHit = a.IsHit }).ToList()
            };
            return (true, null, board);
        }

        public async Task<(bool ok, string? error, BoardDto? board)> GetOpponentAttackBoardAsync(int gameId, string currentUserId)
        {
            var game = await _repo.GetByIdAsync(gameId);
            if (game == null) return (false, "Partida no encontrada", null);
            if (!game.IsUserParticipant(currentUserId)) return (false, "No autorizado", null);
            var opponent = game.CreatorUserId == currentUserId ? game.OpponentUserId : game.CreatorUserId;
            var placements = await _repo.GetPlacementsAsync(gameId, opponent);
            var attacks = await _repo.GetAttacksAsync(gameId, opponent);
            var board = new BoardDto
            {
                Size = 12,
                MyPlacements = placements.Select(p => new PlacementDto { Length = p.Length, StartRow = p.StartRow, StartCol = p.StartCol, Direction = p.Direction }).ToList(),
                MyAttacks = attacks.Select(a => new AttackDto { Row = a.Row, Col = a.Col, IsHit = a.IsHit }).ToList()
            };
            return (true, null, board);
        }

        public async Task<(bool ok, string? error)> PlaceShipAsync(string currentUserId, PlaceShipRequestDto dto)
        {
            var game = await _repo.GetByIdAsync(dto.GameId);
            if (game == null) return (false, "Partida no encontrada");
            if (!game.IsUserParticipant(currentUserId)) return (false, "No autorizado");
            if (game.Status != GameStatus.Setup) return (false, "Fuera de fase de configuración");

            // Límite y catálogo de barcos por usuario: 1x2, 2x3, 1x4, 1x5
            var allowed = new Dictionary<int, int> { [2] = 1, [3] = 2, [4] = 1, [5] = 1 };
            var myPlacements = await _repo.GetPlacementsAsync(dto.GameId, currentUserId);
            if (myPlacements.Count >= 5) return (false, "Ya posicionó todos los barcos");
            if (!allowed.ContainsKey(dto.Length)) return (false, "Tamaño de barco inválido");
            var placedCountByLen = myPlacements.GroupBy(p => p.Length).ToDictionary(g => g.Key, g => g.Count());
            if (placedCountByLen.TryGetValue(dto.Length, out var used) && used >= allowed[dto.Length])
                return (false, "Ya posicionó todos los barcos de ese tamaño");

            // Validaciones de tablero
            var length = dto.Length;
            int endRow = dto.Direction switch
            {
                Direction.Up => dto.StartRow - (length - 1),
                Direction.Down => dto.StartRow + (length - 1),
                _ => dto.StartRow
            };
            int endCol = dto.Direction switch
            {
                Direction.Left => dto.StartCol - (length - 1),
                Direction.Right => dto.StartCol + (length - 1),
                _ => dto.StartCol
            };
            if (dto.StartRow < 0 || dto.StartRow > 11 || dto.StartCol < 0 || dto.StartCol > 11 || endRow < 0 || endRow > 11 || endCol < 0 || endCol > 11)
                return (false, "Posición fuera del tablero");

            // Verifica superposición
            var cells = Cells(dto.StartRow, dto.StartCol, length, dto.Direction);
            foreach (var p in myPlacements)
            {
                var pc = Cells(p.StartRow, p.StartCol, p.Length, p.Direction);
                if (pc.Intersect(cells).Any()) return (false, "El barco se superpone con otro");
            }

            await _repo.AddPlacementAsync(new ShipPlacement
            {
                GameId = dto.GameId,
                UserId = currentUserId,
                Length = dto.Length,
                StartRow = dto.StartRow,
                StartCol = dto.StartCol,
                Direction = dto.Direction,
                CreatedBy = currentUserId
            });

            // Si ambos jugadores ya colocaron 5 barcos, pasa a fase Active y define turno
            var creatorCount = (await _repo.GetPlacementsAsync(dto.GameId, game.CreatorUserId)).Count;
            var opponentCount = (await _repo.GetPlacementsAsync(dto.GameId, game.OpponentUserId)).Count;
            if (creatorCount >= 5 && opponentCount >= 5)
            {
                game.Status = GameStatus.Active;
                game.CurrentTurnUserId = game.CreatorUserId; // el creador inicia
                await _repo.UpdateAsync(game);
            }

            return (true, null);
        }

        public async Task<(bool ok, string? error)> AttackAsync(string currentUserId, AttackRequestDto dto)
        {
            var game = await _repo.GetByIdAsync(dto.GameId);
            if (game == null) return (false, "Partida no encontrada");
            if (!game.IsUserParticipant(currentUserId)) return (false, "No autorizado");
            if (game.Status != GameStatus.Active) return (false, "La partida no está activa");
            if (game.CurrentTurnUserId != currentUserId) return (false, "No es tu turno");
            if (await _repo.HasAttackAtAsync(dto.GameId, currentUserId, dto.Row, dto.Col)) return (false, "Celda ya atacada");

            // Determina si es un hit
            var opponent = game.CreatorUserId == currentUserId ? game.OpponentUserId : game.CreatorUserId;
            var oppPlacements = await _repo.GetPlacementsAsync(dto.GameId, opponent);
            var isHit = oppPlacements.Any(p => Cells(p.StartRow, p.StartCol, p.Length, p.Direction).Contains((dto.Row, dto.Col)));

            await _repo.AddAttackAsync(new Attack
            {
                GameId = dto.GameId,
                AttackerUserId = currentUserId,
                DefenderUserId = opponent,
                Row = dto.Row,
                Col = dto.Col,
                IsHit = isHit,
                CreatedBy = currentUserId
            });

            // Cambia turno
            game.CurrentTurnUserId = opponent;

            // Verifica fin de partida
            var oppCells = oppPlacements.SelectMany(p => Cells(p.StartRow, p.StartCol, p.Length, p.Direction)).ToHashSet();
            var myAttacks = await _repo.GetAttacksAsync(dto.GameId, currentUserId);
            var hitCells = myAttacks.Where(a => a.IsHit).Select(a => (a.Row, a.Col)).ToHashSet();
            if (oppCells.IsSubsetOf(hitCells))
            {
                game.Status = GameStatus.Finished;
                game.FinishedAt = DateTime.UtcNow;
                game.WinnerUserId = currentUserId;
            }

            await _repo.UpdateAsync(game);
            return (true, null);
        }

        public async Task<(bool ok, string? error)> ResignAsync(int gameId, string currentUserId)
        {
            var game = await _repo.GetByIdAsync(gameId);
            if (game == null) return (false, "Partida no encontrada");
            if (!game.IsUserParticipant(currentUserId)) return (false, "No autorizado");
            game.Status = GameStatus.Finished;
            game.FinishedAt = DateTime.UtcNow;
            game.ResignedUserId = currentUserId;
            game.WinnerUserId = game.CreatorUserId == currentUserId ? game.OpponentUserId : game.CreatorUserId;
            await _repo.UpdateAsync(game);
            return (true, null);
        }

        public async Task EvaluateTimeoutAsync(int gameId)
        {
            var game = await _repo.GetByIdAsync(gameId);
            if (game == null || game.Status != GameStatus.Active) return;
            // Si el turno actual no cambia en > 48h, victoria para el otro
            if (game.UpdatedAt.HasValue && (DateTime.UtcNow - game.UpdatedAt.Value).TotalHours > 48)
            {
                var opponent = game.CreatorUserId == game.CurrentTurnUserId ? game.OpponentUserId : game.CreatorUserId;
                game.Status = GameStatus.Finished;
                game.FinishedAt = DateTime.UtcNow;
                game.WinnerUserId = opponent;
                await _repo.UpdateAsync(game);
            }
        }

        private static List<(int Row, int Col)> Cells(int startRow, int startCol, int length, Direction direction)
        {
            var cells = new List<(int, int)>(length);
            for (int i = 0; i < length; i++)
            {
                var r = startRow + (direction == Direction.Down ? i : direction == Direction.Up ? -i : 0);
                var c = startCol + (direction == Direction.Right ? i : direction == Direction.Left ? -i : 0);
                cells.Add((r, c));
            }
            return cells;
        }
    }
}
