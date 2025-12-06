using System;

namespace GameManager.Models
{

    public class Player
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = "";
        public int HoursPlayed { get; set; } = 0;
        public int HighScore { get; set; } = 0;

        public override string ToString()
        {
            return $"{Id} | {Username} | Hours: {HoursPlayed} | Score: {HighScore}";
        }
    }
}
