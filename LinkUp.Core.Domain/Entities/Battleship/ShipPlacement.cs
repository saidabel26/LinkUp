using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Domain.Battleship
{
    public class ShipPlacement : Common.BaseEntity
    {
        public int GameId { get; set; }
        public BattleshipGame? Game { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Length { get; set; }
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public Direction Direction { get; set; }
    }
}
