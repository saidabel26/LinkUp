namespace LinkUp.Core.Application.Dtos.Battleship
{
    public class AttackDto
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsHit { get; set; }
    }
}