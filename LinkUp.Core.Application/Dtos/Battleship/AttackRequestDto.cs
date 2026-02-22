namespace LinkUp.Core.Application.Dtos.Battleship
{
    public class AttackRequestDto
    {
        public int GameId { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}