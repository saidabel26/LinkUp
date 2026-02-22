namespace LinkUp.Core.Application.Dtos.Battleship
{
    public class GameHistoryDto
    {
        public int Id { get; set; }
        public string OpponentUserId { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public double DurationHours { get; set; }
        public bool Won { get; set; }
        public string WinnerUserId { get; set; } = string.Empty;
    }
}