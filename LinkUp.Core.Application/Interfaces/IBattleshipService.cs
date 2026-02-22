using LinkUp.Core.Application.Dtos.Battleship;

namespace LinkUp.Core.Application.Interfaces
{
    public interface IBattleshipService
    {
        Task<(bool ok, string? error, int? gameId)> StartGameAsync(string currentUserId, string opponentUserId);
        Task<IReadOnlyList<GameSummaryDto>> GetActiveGamesAsync(string currentUserId);
        Task<IReadOnlyList<GameHistoryDto>> GetHistoryAsync(string currentUserId);
        Task<(bool ok, string? error, GameSummaryDto? game)> GetSummaryAsync(int gameId, string currentUserId);
        Task<(bool ok, string? error, BoardDto? board)> GetMyBoardAsync(int gameId, string currentUserId);
        Task<(bool ok, string? error, BoardDto? board)> GetOpponentAttackBoardAsync(int gameId, string currentUserId);
        Task<(bool ok, string? error)> PlaceShipAsync(string currentUserId, PlaceShipRequestDto dto);
        Task<(bool ok, string? error)> AttackAsync(string currentUserId, AttackRequestDto dto);
        Task<(bool ok, string? error)> ResignAsync(int gameId, string currentUserId);
        Task EvaluateTimeoutAsync(int gameId);
    }
}
