using System;

namespace GameManager.Models
{
    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = "";
        public int HoursPlayed { get; set; }
        public int HighScore { get; set; }
        public string Team { get; set; } = "";
        public double Rating { get; set; }

        public override string ToString()
        {
            return $"{Id} | {Username} | Hours: {HoursPlayed} | Score: {HighScore} | Team: {Team} | Rating: {Rating}";
        }
    }
}
