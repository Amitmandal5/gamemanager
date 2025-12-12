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
                Console.WriteLine("7. Update Player");
                Console.WriteLine("0. Exit");
                Console.Write("Choose: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddPlayerFlow(); break;
                    case "2": ListPlayers(); break;
                    case "3": SearchPlayer(); break;
                    case "4": SortingMenu(); break;
                    case "5": GenerateReport(); break;
                    case "6": ExportCSV(); break;
                    case "7": UpdatePlayerFlow(); break;
                    case "0": run = false; break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        // ---------------- ADD PLAYER ----------------
        private void AddPlayerFlow()
        {
            Console.Write("Username: ");
            string username = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(username)) return;

            Console.Write("Hours played: ");
            if (!int.TryParse(Console.ReadLine(), out int hours) || hours < 0) return;

            Console.Write("High score: ");
            if (!int.TryParse(Console.ReadLine(), out int score) || score < 0) return;

            Console.Write("Team: ");
            string team = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(team)) return;

            Console.Write("Rating: ");
            if (!double.TryParse(Console.ReadLine(), out double rating)) return;

            Player p = _repo.AddPlayer(username, hours, score, team, rating);
            Console.WriteLine("Player added:");
            Console.WriteLine(p);
        }

        // ---------------- LIST ----------------
        private void ListPlayers()
        {
            var players = _repo.GetAllPlayers();
            if (players.Count == 0)
            {
                Console.WriteLine("No players found.");
                return;
            }

            foreach (var p in players)
                Console.WriteLine(p);
        }

        // ---------------- SEARCH ----------------
        private void SearchPlayer()
        {
            Console.Write("Enter Player ID or Username: ");
            string input = Console.ReadLine() ?? "";

            if (Guid.TryParse(input, out Guid id))
            {
                var player = _repo.GetById(id);

                if (player == null)
                    Console.WriteLine("Player not found.");
                else
                    Console.WriteLine(player);

                return;
            }

            var list = _repo.Search(input);
            if (list.Count == 0)
            {
                Console.WriteLine("Player not found.");
                return;
            }

            foreach (var p in list)
                Console.WriteLine(p);
        }

        // ---------------- SORT ----------------
        private void SortingMenu()
        {
            Console.WriteLine("\n1. By Rating");
            Console.WriteLine("2. By High Score");
            Console.WriteLine("3. By Hours Played");
            Console.Write("Choose: ");

            switch (Console.ReadLine())
            {
                case "1":
                    foreach (var p in _repo.SortByRating()) Console.WriteLine(p);
                    break;
                case "2":
                    foreach (var p in _repo.SortByHighScore()) Console.WriteLine(p);
                    break;
                case "3":
                    foreach (var p in _repo.SortByHours()) Console.WriteLine(p);
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }

        // ---------------- UPDATE ----------------
        private void UpdatePlayerFlow()
        {
            Console.Write("Enter Player ID: ");
            if (!Guid.TryParse(Console.ReadLine(), out Guid id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }

            var player = _repo.GetById(id);
            if (player == null)
            {
                Console.WriteLine("Player not found.");
                return;
            }

            Console.WriteLine(player);

            Console.Write("New hours: ");
            if (!int.TryParse(Console.ReadLine(), out int hours)) return;

            Console.Write("New score: ");
            if (!int.TryParse(Console.ReadLine(), out int score)) return;

            Console.Write("New team: ");
            string team = Console.ReadLine() ?? "";

            Console.Write("New rating: ");
            if (!double.TryParse(Console.ReadLine(), out double rating)) return;

            bool ok = _repo.UpdatePlayer(id, hours, score, team, rating);
            Console.WriteLine(ok ? "Player updated." : "Update failed.");
        }

        // ---------------- REPORT ----------------
        private void GenerateReport()
        {
            string path = _repo.GenerateReport();
            Console.WriteLine("Report saved at: " + path);
        }

        // ---------------- CSV ----------------
        private void ExportCSV()
        {
            string path = _repo.ExportCSV();
            Console.WriteLine("CSV saved at: " + path);
        }
    }
}
