using System;

namespace GameManager.Models
{
    // ProPlayer inherits from Player and changes the Rating() behaviour.
    public class ProPlayer : Player
    {
        public string TeamName { get; set; } = "No Team";

        // Pro players get a small rating bonus to show polymorphism in action.
        public override double Rating()
        {
            double baseRating = base.Rating();
            double bonus = 20.0; // simple fixed bonus for pros
            return baseRating + bonus;
        }
    }
}
