using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GameManager.Models;

namespace GameManager.Services
{
    // Repository responsible for storing and managing players.
    // Supports JSON saving/loading, logging, search and sorting.
    public class PlayerRepository
    {
        private List<Player> _players = new List<Player>();
        private readonly Logger _logger;
        private readonly string _dataFilePath;

        public PlayerRepository(Logger logger, string dataFilePath)
        {
            _logger = logger;
            _dataFilePath = dataFilePath;

            // Make sure the folder exists for the data file.
            string? folder = Path.GetDirectoryName(_dataFilePath);
            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        // ----------------- JSON persistence -----------------

        public void LoadFromFile()
        {
            if (!File.Exists(_dataFilePath))
            {
                _logger.Info($"Data file '{_dataFilePath}' not found. Starting with empty list.");
                return;
            }

            try
            {
                string json = File.ReadAllText(_dataFilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    _logger.Warning($"Data file '{_dataFilePath}' is empty. No players loaded.");
                    return;
                }

                var loaded = JsonSerializer.Deserialize<List<Player>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (loaded == null)
                {
                    _logger.Warning("Could not deserialize players. Starting with empty list.");
                    return;
                }

                _players = loaded;
                _logger.Info($"Loaded {_players.Count} player(s) from '{_dataFilePath}'.");
            }
            catch (JsonException ex)
            {
                _logger.Error("JSON in data file is not valid: " + ex.Message);
            }
            catch (Exception ex)
            {
                _logger.Error("Error while reading data file: " + ex.Message);
            }
        }

        public void SaveToFile()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                string json = JsonSerializer.Serialize(_players, options);
                File.WriteAllText(_dataFilePath, json);

                _logger.Info($"Saved {_players.Count} player(s) to '{_dataFilePath}'.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error while saving data file: " + ex.Message);
            }
        }

        // ----------------- Basic operations -----------------

        // Add a new player with validation.
        // isPro = true means create a ProPlayer instead of normal Player.
        public Player AddPlayer(string username, bool isPro)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Username cannot be empty.");

            // Avoid duplicate usernames (case insensitive).
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
                newPlayer = new Player();
            }

            newPlayer.Username = username.Trim();

            _players.Add(newPlayer);

            _logger.Info($"Added player '{newPlayer.Username}' (ID: {newPlayer.Id}) (Pro: {isPro})");

            // Auto-save after adding a player
            SaveToFile();

            return newPlayer;
        }

        // Overload (not used by menu, but kept in case).
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
        public Player? GetById(Guid id)
        {
            foreach (var p in _players)
            {
                if (p.Id == id)
                    return p;
            }

            return null;
        }

        // Update stats for a player.
        // hoursToAdd: how many hours to add to existing value
        // newHighScore: optional, only used if it has a value
        public bool UpdateStats(Guid playerId, int hoursToAdd, int? newHighScore)
        {
            Player? target = GetById(playerId);
            if (target == null)
            {
                _logger.Warning($"Tried to update stats for ID {playerId} but player was not found.");
                return false; // player not found
            }

            if (hoursToAdd < 0)
                throw new Exception("Hours to add cannot be negative.");

            if (newHighScore.HasValue && newHighScore.Value < 0)
                throw new Exception("High score cannot be negative.");

            target.HoursPlayed += hoursToAdd;

            if (newHighScore.HasValue && newHighScore.Value > target.HighScore)
            {
                target.HighScore = newHighScore.Value;
            }

            _logger.Info($"Updated stats for '{target.Username}' (ID: {target.Id}). " +
                         $"Hours: {target.HoursPlayed}, Score: {target.HighScore}");

            // Auto-save after updating stats
            SaveToFile();

            return true;
        }

        // ----------------- Search and algorithms -----------------

        // Search by username (case-insensitive, partial match).
        // Simple linear search.
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

        // Manual insertion sort by high score (descending).
        public List<Player> GetTopByHighScore(int topN)
        {
            if (topN <= 0)
                return new List<Player>();

            List<Player> copy = new List<Player>(_players);

            for (int i = 1; i < copy.Count; i++)
            {
                Player current = copy[i];
                int j = i - 1;

                while (j >= 0 && copy[j].HighScore < current.HighScore)
                {
                    copy[j + 1] = copy[j];
                    j--;
                }

                copy[j + 1] = current;
            }

            if (topN > copy.Count)
                topN = copy.Count;

            return copy.GetRange(0, topN);
        }

        // Built-in sort example: most active by hours played.
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
