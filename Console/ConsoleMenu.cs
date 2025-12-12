using System;
using System.Collections.Generic;
using GameManager.Services;
using GameManager.Models;

namespace GameManager.ConsoleApp
{
    // Simple text menu to interact with the player system.
    public class ConsoleMenu
    {
        private readonly PlayerRepository _repo;

        public ConsoleMenu(PlayerRepository repo)
        {
            _repo = repo;
        }

        public void Show()
        {
            bool running = true;

            while (running)
            {
                Console.WriteLine("\n=== Game Manager ===");
                Console.WriteLine("1. Add Player");
                Console.WriteLine("2. List All Players");
                Console.WriteLine("3. Update Player Stats");
                Console.WriteLine("4. Search Player (ID or Username)");
                Console.WriteLine("5. Reports (Top / Active)");
                Console.WriteLine("0. Exit");
                Console.Write("Choose option: ");

                string? input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddPlayerFlow();
                        break;

                    case "2":
                        ShowPlayers();
                        break;

                    case "3":
                        UpdatePlayerStatsFlow();
                        break;

                    case "4":
                        SearchPlayerFlow();
                        break;

                    case "5":
                        ReportsFlow();
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }
            }
        }

        // -------- Add Player (improved) --------
        private void AddPlayerFlow()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            Console.Write("Is this a pro player? (y/n): ");
            string proInput = (Console.ReadLine() ?? "").Trim().ToLower();
            bool isPro = (proInput == "y" || proInput == "yes");

            // Starting hours
            Console.Write("Starting hours played (leave empty for 0): ");
            string? hoursText = Console.ReadLine();
            int startingHours = 0;
            if (!string.IsNullOrWhiteSpace(hoursText))
            {
                if (!int.TryParse(hoursText, out startingHours) || startingHours < 0)
                {
                    Console.WriteLine("Hours must be a non-negative whole number.");
                    return;
                }
            }

            // Starting high score
            Console.Write("Starting high score (leave empty for 0): ");
            string? scoreText = Console.ReadLine();
            int startingScore = 0;
            if (!string.IsNullOrWhiteSpace(scoreText))
            {
                if (!int.TryParse(scoreText, out startingScore) || startingScore < 0)
                {
                    Console.WriteLine("High score must be a non-negative whole number.");
                    return;
                }
            }

            // Team for pro players
            string teamName = "No Team";
            if (isPro)
            {
                Console.Write("Team name (optional, leave empty for 'No Team'): ");
                string? inputTeam = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(inputTeam))
                {
                    teamName = inputTeam.Trim();
                }
            }

            try
            {
                // Create the player
                Player p = _repo.AddPlayer(username, isPro);

                // Set initial stats (this will auto-save as well)
                if (startingHours > 0 || startingScore > 0)
                {
                    _repo.UpdateStats(p.Id, startingHours, startingScore);
                    p = _repo.GetById(p.Id)!;
                }

                // If pro, set team name
                if (isPro && p is ProPlayer pro)
                {
                    pro.TeamName = teamName;
                    _repo.SaveToFile();  // save again to record team name
                }

                Console.WriteLine("Player added successfully!");
                Console.WriteLine(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add player: " + ex.Message);
            }
        }

        // -------- List players --------
        private void ShowPlayers()
        {
            var players = _repo.GetAllPlayers();

            if (players.Count == 0)
            {
                Console.WriteLine("No players found.");
                return;
            }

            foreach (var p in players)
            {
                Console.WriteLine(p);
            }
        }

        // -------- Update stats --------
        private void UpdatePlayerStatsFlow()
        {
            Console.Write("Enter player ID: ");
            string? idText = Console.ReadLine();

            if (!Guid.TryParse(idText, out Guid id))
            {
                Console.WriteLine("Invalid ID format. It should be a GUID.");
                return;
            }

            Console.Write("Hours to add (can be 0): ");
            string? hoursText = Console.ReadLine();
            if (!int.TryParse(hoursText, out int hoursToAdd))
            {
                Console.WriteLine("Hours must be a whole number.");
                return;
            }

            Console.Write("New high score (leave empty to keep current): ");
            string? scoreText = Console.ReadLine();
            int? newHighScore = null;

            if (!string.IsNullOrWhiteSpace(scoreText))
            {
                if (!int.TryParse(scoreText, out int parsedScore))
                {
                    Console.WriteLine("High score must be a number.");
                    return;
                }
                newHighScore = parsedScore;
            }

            try
            {
                bool updated = _repo.UpdateStats(id, hoursToAdd, newHighScore);

                if (!updated)
                {
                    Console.WriteLine("Player not found with that ID.");
                    return;
                }

                Console.WriteLine("Player stats updated successfully.");
                Player? p = _repo.GetById(id);
                if (p != null)
                {
                    Console.WriteLine(p);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to update stats: " + ex.Message);
            }
        }

        // -------- Search (ID or username in one screen) --------
        private void SearchPlayerFlow()
        {
            Console.Write("Enter player ID or username/part of username: ");
            string input = (Console.ReadLine() ?? "").Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("You must type something.");
                return;
            }

            // Try to treat input as ID first
            if (Guid.TryParse(input, out Guid id))
            {
                Player? byId = _repo.GetById(id);
                if (byId == null)
                {
                    Console.WriteLine("No player found with that ID.");
                }
                else
                {
                    Console.WriteLine("Player found:");
                    Console.WriteLine(byId);
                }
                return;
            }

            // Otherwise treat as username search
            List<Player> results = _repo.SearchByUsername(input);

            if (results.Count == 0)
            {
                Console.WriteLine("No players matched your search.");
                return;
            }

            Console.WriteLine($"Found {results.Count} player(s):");
            foreach (var p in results)
            {
                Console.WriteLine(p);
            }
        }

        // -------- Reports (sorting demo) --------
        private void ReportsFlow()
        {
            Console.WriteLine("\n=== Reports ===");
            Console.Write("How many top players do you want to see? ");
            string? input = Console.ReadLine();

            if (!int.TryParse(input, out int n) || n <= 0)
            {
                Console.WriteLine("Please enter a positive whole number.");
                return;
            }

            var topScores = _repo.GetTopByHighScore(n);
            var mostActive = _repo.GetMostActiveByHours(n);

            Console.WriteLine("\n--- Top Players by High Score (manual sort) ---");
            if (topScores.Count == 0)
            {
                Console.WriteLine("No players available.");
            }
            else
            {
                foreach (var p in topScores)
                {
                    Console.WriteLine(p);
                }
            }

            Console.WriteLine("\n--- Most Active Players by Hours (built-in sort) ---");
            if (mostActive.Count == 0)
            {
                Console.WriteLine("No players available.");
            }
            else
            {
                foreach (var p in mostActive)
                {
                    Console.WriteLine(p);
                }
            }
        }
    }
}
