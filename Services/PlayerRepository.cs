using System;
using System.Collections.Generic;
using System.Linq;
using GameManager.Models;

namespace GameManager.Services
{
    // Simple repository responsible for storing and managing players.
    // It keeps data in memory for now. Later we will add saving/loading to file.
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

        // Find a player by their Guid ID. Returns null if not found.
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
                return false; 
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

        // ----------- NEW: Search and Reports -----------------

      
        // This is a simple linear search algorithm.
        public List<Player> SearchByUsername(string term)
        {
            List<Player> results = new List<Player>();

            if (string.IsNullOrWhiteSpace(term))
                return results;

            string lowerTerm = term.Trim().ToLower();

            foreach (var p in _players)
            {
                string name = (p.Username ?? "").ToLower();
                if (name.Contains(lowerTerm))
                {
                    results.Add(p);
                }
            }

            return results;
        }

        // Get top N players sorted by high score (descending),
        // using a manual insertion sort on a copy of the list.
        public List<Player> GetTopByHighScore(int topN)
        {
            if (topN <= 0)
                return new List<Player>();

            // Work on a copy so we do not break original order.
            List<Player> copy = new List<Player>(_players);

            // Insertion sort (descending by HighScore).
            for (int i = 1; i < copy.Count; i++)
            {
                Player current = copy[i];
                int j = i - 1;

                // move items that are smaller to the right
                while (j >= 0 && copy[j].HighScore < current.HighScore)
                {
                    copy[j + 1] = copy[j];
                    j--;
                }

                copy[j + 1] = current;
            }

            // Take top N items (or all if fewer).
            if (topN > copy.Count)
                topN = copy.Count;

            return copy.GetRange(0, topN);
        }

        // Get top N most active players (by HoursPlayed) using built-in sort.
        
        public List<Player> GetMostActiveByHours(int topN)
        {
            if (topN <= 0)
                return new List<Player>();

            var sorted = _players
                .OrderByDescending(p => p.HoursPlayed)
                .ToList();

            if (topN > sorted.Count)
                topN = sorted.Count;

            return sorted.GetRange(0, topN);
        }
    }
}
