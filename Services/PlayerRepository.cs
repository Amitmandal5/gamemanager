using System;
using System.Collections.Generic;
using GameManager.Models;

namespace GameManager.Services
{
    // Simple repository responsible for storing and managing players.
    // Right now it only keeps data in memory. We can add saving/loading later.
    public class PlayerRepository
    {
        private List<Player> _players = new List<Player>();

        // Add a new player with validation.
        public Player AddPlayer(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new Exception("Username cannot be empty.");

            // Avoid duplicate usernames.
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

        // Return all players.
        public List<Player> GetAllPlayers()
        {
            return _players;
        }
    }
}
