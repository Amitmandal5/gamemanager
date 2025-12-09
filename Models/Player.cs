using System;

namespace GameManager.Models
{
    // Basic player model used for storing player information.
    // Implements IIdentifiable to show interface usage.
    public class Player : IIdentifiable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = "";
        public int HoursPlayed { get; set; } = 0;
        public int HighScore { get; set; } = 0;

        // Virtual method so child classes can change the formula (polymorphism).
        public virtual double Rating()
        {
            // Very simple rating formula: 70% high score + 30% hours.
            double scorePart = HighScore * 0.7;
            double hoursPart = HoursPlayed * 0.3;
            return scorePart + hoursPart;
        }

        public override string ToString()
        {
            // ToString shows rating as well. For ProPlayer, overridden Rating()
            // will be called automatically (polymorphism).
            return $"{Id} | {Username} | Hours: {HoursPlayed} | Score: {HighScore} | Rating: {Rating():0.0}";
        }
    }
}
