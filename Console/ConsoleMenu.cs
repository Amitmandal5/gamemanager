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
    }
}
