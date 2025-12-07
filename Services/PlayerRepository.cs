using System;
using System.Collections.Generic;
using GameManager.Models;

namespace GameManager.Services
{
    // Simple repository responsible for storing and managing players.
    // Right now it only keeps data in memory. We will add saving/loading later.
    public class PlayerRepository
    {
        private List<Player> _players = new List<Player>();

        // Add a new player with validation.
        public Player AddPlayer(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Username cannot be empty.");

            // Avoid duplicate usernames (case insensitive).
            foreach (var p in _players)
            {
                if (p.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("This username already exists.");
            }

            Player newPlayer = new Player
            {
                Username = username.Trim()
            };

            _players.Add(newPlayer);
            return newPlayer;
        }

        // Get all players.
        public List<Player> GetAllPlayers()
        {
            return _players;
        }

        // Find a player by their Guid ID.
        public Player GetById(Guid id)
        {
            foreach (var p in _players)
            {
                if (p.Id == id)
                    return p;
            }

            return null;
        }

        // Update stats for a player:
        // - hoursToAdd: how many hours to add to existing value
        // - newHighScore: optional, only used if it has a value
        public bool UpdateStats(Guid playerId, int hoursToAdd, int? newHighScore)
        {
            Player target = GetById(playerId);
            if (target == null)
            {
                return false; // player not found
            }

            if (hoursToAdd < 0)
                throw new Exception("Hours to add cannot be negative.");

            if (newHighScore.HasValue && newHighScore.Value < 0)
                throw new Exception("High score cannot be negative.");

            // add hours
            target.HoursPlayed += hoursToAdd;

            // only update high score if provided and actually higher
            if (newHighScore.HasValue && newHighScore.Value > target.HighScore)
            {
                target.HighScore = newHighScore.Value;
            }

            return true;
        }
    }
}
