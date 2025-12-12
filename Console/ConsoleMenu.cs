using System;
using GameManager.Services;
using GameManager.Models;

namespace GameManager.ConsoleApp
{
    public class ConsoleMenu
    {
        private readonly PlayerRepository _repo;

        public ConsoleMenu(PlayerRepository repo)
        {
            _repo = repo;
        }

        public void Show()
        {
            bool run = true;

            while (run)
            {
                Console.WriteLine("\n=== GAME MANAGER ===");
                Console.WriteLine("1. Add Player");
                Console.WriteLine("2. List All Players");
                Console.WriteLine("3. Search Player (ID or Username)");
                Console.WriteLine("4. Sorting Options");
                Console.WriteLine("5. Generate Report");
                Console.WriteLine("6. Export CSV");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddPlayerFlow();
                        break;

                    case "2":
                        ListPlayers();
                        break;

                    case "3":
                        SearchPlayer();
                        break;

                    case "4":
                        SortingMenu();
                        break;

                    case "5":
                        GenerateReport();
                        break;

                    case "6":
                        ExportCSV();
                        break;

                    case "0":
                        run = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        // ---------------- ADD PLAYER ----------------
        private void AddPlayerFlow()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty.");
                return;
            }

            Console.Write("Hours played: ");
            if (!int.TryParse(Console.ReadLine(), out int hours) || hours < 0)
            {
                Console.WriteLine("Invalid hours. Please enter a non-negative number.");
                return;
            }

            Console.Write("High score: ");
            if (!int.TryParse(Console.ReadLine(), out int score) || score < 0)
            {
                Console.WriteLine("Invalid score. Please enter a non-negative number.");
                return;
            }

            Console.Write("Team: ");
            string team = Console.ReadLine() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(team))
            {
                Console.WriteLine("Team cannot be empty.");
                return;
            }

            Console.Write("Rating: ");
            if (!double.TryParse(Console.ReadLine(), out double rating))
            {
                Console.WriteLine("Invalid rating. Please enter a number.");
                return;
            }

            try
            {
                Player p = _repo.AddPlayer(username, hours, score, team, rating);
                Console.WriteLine("\nPlayer added:");
                Console.WriteLine(p);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not add player: " + ex.Message);
            }
        }

        // ---------------- LIST PLAYERS ----------------
        private void ListPlayers()
        {
            var players = _repo.GetAllPlayers();

            if (players.Count == 0)
            {
                Console.WriteLine("No players available.");
                return;
            }

            foreach (var p in players)
            {
                Console.WriteLine(p);
            }
        }

        // ---------------- SEARCH PLAYER ----------------
        private void SearchPlayer()
        {
            Console.Write("Enter Player ID or Username: ");
            string input = Console.ReadLine() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Search term cannot be empty.");
                return;
            }

            // Try search by ID first
            if (Guid.TryParse(input, out Guid id))
            {
                var player = _repo.GetById(id);
                if (player == null)
                {
                    Console.WriteLine("Player not found.");
                }
                else
                {
                    Console.WriteLine(player);
                }

                return;
            }

            // Fallback to search by username
            var list = _repo.Search(input);
            if (list.Count == 0)
            {
                Console.WriteLine("Player not found.");
                return;
            }

            foreach (var p in list)
            {
                Console.WriteLine(p);
            }
        }

        // ---------------- SORTING MENU ----------------
        private void SortingMenu()
        {
            Console.WriteLine("\n--- Sorting Options ---");
            Console.WriteLine("1. Sort by Rating");
            Console.WriteLine("2. Sort by High Score");
            Console.WriteLine("3. Sort by Hours Played");
            Console.Write("Choose: ");

            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\n--- Players Sorted by Rating (Highest First) ---");
                    foreach (var p in _repo.SortByRating())
                    {
                        Console.WriteLine(p);
                    }
                    break;

                case "2":
                    Console.WriteLine("\n--- Players Sorted by High Score (Highest First) ---");
                    foreach (var p in _repo.SortByHighScore())
                    {
                        Console.WriteLine(p);
                    }
                    break;

                case "3":
                    Console.WriteLine("\n--- Players Sorted by Hours Played (Most Active First) ---");
                    foreach (var p in _repo.SortByHours())
                    {
                        Console.WriteLine(p);
                    }
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        // ---------------- REPORT ----------------
        private void GenerateReport()
        {
            try
            {
                string path = _repo.GenerateReport();
                Console.WriteLine("Report generated at: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to generate report: " + ex.Message);
            }
        }

        // ---------------- EXPORT CSV ----------------
        private void ExportCSV()
        {
            try
            {
                string path = _repo.ExportCSV();
                Console.WriteLine("CSV exported at: " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to export CSV: " + ex.Message);
            }
        }
    }
}
