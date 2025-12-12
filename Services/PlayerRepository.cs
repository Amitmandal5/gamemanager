using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using GameManager.Models;

namespace GameManager.Services
{
    public class PlayerRepository
    {
        private List<Player> _players = new();
        private readonly Logger _logger;
        private readonly string _dataFilePath;

        public PlayerRepository(Logger logger, string dataFilePath)
        {
            _logger = logger;
            _dataFilePath = dataFilePath;

            string? folder = Path.GetDirectoryName(_dataFilePath);
            if (!string.IsNullOrWhiteSpace(folder) && !Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        // ---------------- LOAD + SAVE JSON ----------------
        public void LoadFromFile()
        {
            if (!File.Exists(_dataFilePath))
            {
                _logger.Info("No player file found. Starting empty.");
                return;
            }

            try
            {
                string json = File.ReadAllText(_dataFilePath);
                var data = JsonSerializer.Deserialize<List<Player>>(json);

                if (data != null)
                {
                    _players = data;
                    _logger.Info($"Loaded {_players.Count} players.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error loading JSON: " + ex.Message);
            }
        }

        public void SaveToFile()
        {
            try
            {
                string json = JsonSerializer.Serialize(
                    _players,
                    new JsonSerializerOptions { WriteIndented = true }
                );

                File.WriteAllText(_dataFilePath, json);
                _logger.Info("Saved players successfully.");
            }
            catch (Exception ex)
            {
                _logger.Error("Error saving JSON: " + ex.Message);
            }
        }

        // ---------------- ADD PLAYER ----------------
        public Player AddPlayer(string username, int hours, int score, string team, double rating)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(team))
                throw new Exception("Team name is required.");

            Player p = new Player
            {
                Username = username.Trim(),
                HoursPlayed = hours,
                HighScore = score,
                Team = team.Trim(),
                Rating = rating
            };

            _players.Add(p);
            SaveToFile();
            _logger.Info("Added new player: " + username);

            return p;
        }

        // ---------------- GET ALL PLAYERS (FIX ADDED HERE) ----------------
        public List<Player> GetAllPlayers()
        {
            return _players;
        }

        // ---------------- SEARCH ----------------
        public Player? GetById(Guid id)
        {
            return _players.FirstOrDefault(p => p.Id == id);
        }

        public List<Player> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return new();

            term = term.ToLower();

            return _players
                .Where(p => p.Username.ToLower().Contains(term))
                .ToList();
        }

        // ---------------- SORT BY RATING (Insertion Sort) ----------------
        public List<Player> SortByRating()
        {
            List<Player> arr = new(_players);

            for (int i = 1; i < arr.Count; i++)
            {
                Player current = arr[i];
                int j = i - 1;

                while (j >= 0 && arr[j].Rating < current.Rating)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }

                arr[j + 1] = current;
            }

            return arr;
        }

        // ---------------- SORT BY HIGH SCORE (Insertion Sort) ----------------
        public List<Player> SortByHighScore()
        {
            List<Player> arr = new(_players);

            for (int i = 1; i < arr.Count; i++)
            {
                Player current = arr[i];
                int j = i - 1;

                while (j >= 0 && arr[j].HighScore < current.HighScore)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }

                arr[j + 1] = current;
            }

            return arr;
        }

        // ---------------- SORT BY HOURS PLAYED (Built-in) ----------------
        public List<Player> SortByHours()
        {
            return _players.OrderByDescending(p => p.HoursPlayed).ToList();
        }

        // ---------------- REPORT GENERATION ----------------
        public string GenerateReport()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(_dataFilePath)!,
                "report.txt"
            );

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("=== PLAYER REPORT ===");
                writer.WriteLine($"Generated: {DateTime.Now}");
                writer.WriteLine();

                writer.WriteLine("--- MOST ACTIVE PLAYERS (Hours Played) ---");
                foreach (var p in SortByHours())
                    writer.WriteLine(p);

                writer.WriteLine();
                writer.WriteLine("--- TOP RATED PLAYERS ---");
                foreach (var p in SortByRating())
                    writer.WriteLine(p);

                writer.WriteLine();
                writer.WriteLine("--- TOP HIGH SCORE PLAYERS ---");
                foreach (var p in SortByHighScore())
                    writer.WriteLine(p);
            }

            _logger.Info("Report generated.");
            return path;
        }

        // ---------------- EXPORT CSV ----------------
        public string ExportCSV()
        {
            string path = Path.Combine(
                Path.GetDirectoryName(_dataFilePath)!,
                "report.csv"
            );

            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine("Id,Username,HoursPlayed,HighScore,Rating");

                foreach (var p in SortByHours())
                {
                    writer.WriteLine(
                        $"{p.Id},{p.Username},{p.HoursPlayed},{p.HighScore},{p.Rating}"
                    );
                }
            }

            _logger.Info("CSV exported.");
            return path;
        }
    }
}
