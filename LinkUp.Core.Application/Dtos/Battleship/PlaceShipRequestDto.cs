using LinkUp.Core.Domain.Common.Enums;

namespace LinkUp.Core.Application.Dtos.Battleship
{
    public class PlaceShipRequestDto
    {
        public int GameId { get; set; }
        public int Length { get; set; }
        public int StartRow { get; set; }
        public int StartCol { get; set; }
        public Direction Direction { get; set; }
    }
}