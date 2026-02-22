using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Domain.Battleship
{
    public class BattleshipGame : Common.BaseEntity
    {
        public string CreatorUserId { get; set; } = string.Empty;
        public string OpponentUserId { get; set; } = string.Empty;
        public GameStatus Status { get; set; } = GameStatus.Setup;
        public string? CurrentTurnUserId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? WinnerUserId { get; set; }
        public string? ResignedUserId { get; set; }

        public ICollection<ShipPlacement> Placements { get; set; } = new List<ShipPlacement>();
        public ICollection<Attack> Attacks { get; set; } = new List<Attack>();

        public bool IsUserParticipant(string userId) => CreatorUserId == userId || OpponentUserId == userId;
    }
}
