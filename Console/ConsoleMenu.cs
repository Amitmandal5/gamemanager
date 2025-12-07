using System;
using GameManager.Services;
using GameManager.Models;

namespace GameManager.ConsoleApp
{
    // Simple text menu to interact with the player system.
    public class ConsoleMenu
    {
        private PlayerRepository _repo;

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
                Console.WriteLine("3. Update Player Stats"); // NEW OPTION
                Console.WriteLine("0. Exit");
                Console.Write("Choose option: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddPlayerFlow();
                        break;

                    case "2":
                        ShowPlayers();
                        break;

                    case "3":
                        UpdatePlayerStatsFlow(); // NEW FLOW
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

        private void AddPlayerFlow()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            try
            {
                Player p = _repo.AddPlayer(username);
                Console.WriteLine("Player added successfully!");
                Console.WriteLine(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to add player: " + ex.Message);
            }
        }

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

        // NEW: Update stats (hours and high score) for an existing player.
        private void UpdatePlayerStatsFlow()
        {
            Console.Write("Enter player ID: ");
            string idText = Console.ReadLine();

            if (!Guid.TryParse(idText, out Guid id))
            {
                Console.WriteLine("Invalid ID format. It should be a GUID.");
                return;
            }

            // Ask how many hours to add
            Console.Write("Hours to add (can be 0): ");
            string hoursText = Console.ReadLine();
            if (!int.TryParse(hoursText, out int hoursToAdd))
            {
                Console.WriteLine("Hours must be a whole number.");
                return;
            }

            // Ask for new high score, but allow user to skip
            Console.Write("New high score (leave empty to keep current): ");
            string scoreText = Console.ReadLine();
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
                Player p = _repo.GetById(id);
                Console.WriteLine(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to update stats: " + ex.Message);
            }
        }
    }
}
