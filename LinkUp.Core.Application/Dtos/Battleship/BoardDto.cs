namespace LinkUp.Core.Application.Dtos.Battleship
{
    public class BoardDto
    {
        public int Size { get; set; } = 12;
        public List<PlacementDto> MyPlacements { get; set; } = new();
        public List<AttackDto> MyAttacks { get; set; } = new();
    }
}