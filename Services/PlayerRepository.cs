using System;
using System.Collections.Generic;
using System.Linq;
using GameManager.Models;

namespace GameManager.Services
{
   
    public class PlayerRepository
    {
        private List<Player> _players = new List<Player>();

       
        public Player AddPlayer(string username, bool isPro)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Username cannot be empty.");

            foreach (var p in _players)
            {
                if (p.Username.Equals(username, StringComparison.OrdinalIgnoreCase))
                    throw new Exception("This username already exists.");
            }

            Player newPlayer;

            if (isPro)
            {
            
                newPlayer = new ProPlayer();
            }
            else
            {
                // create a normal Player
                newPlayer = new Player();
            }

            newPlayer.Username = username.Trim();

            _players.Add(newPlayer);
            return newPlayer;
        }

        // Overload for backward compatibility if needed.
        public Player AddPlayer(string username)
        {
            return AddPlayer(username, false);
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

        // Search by username (case-insensitive, partial match).
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
