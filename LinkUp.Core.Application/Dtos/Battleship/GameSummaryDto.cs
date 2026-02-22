using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Dtos.Battleship
{
    public class GameSummaryDto
    {
        public int Id { get; set; }
        public string CreatorUserId { get; set; } = string.Empty;
        public string OpponentUserId { get; set; } = string.Empty;
        public GameStatus Status { get; set; }
        public string? CurrentTurnUserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? WinnerUserId { get; set; }
        public string? ResignedUserId { get; set; }
    }
}