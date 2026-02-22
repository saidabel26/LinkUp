using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Domain.Battleship
{
    public class Attack : Common.BaseEntity
    {
        public int GameId { get; set; }
        public BattleshipGame? Game { get; set; }
        public string AttackerUserId { get; set; } = string.Empty;
        public string DefenderUserId { get; set; } = string.Empty;
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsHit { get; set; }
    }
}
