using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Typeracer.Models
{
    public class Leaderboard
    {
        private readonly string LeaderboardFile;
        private List<Player> players;

        public Leaderboard(IWebHostEnvironment env)
        {
            LeaderboardFile = Path.Combine(env.ContentRootPath, "leaderboard.json");
            players = LoadLeaderboard();
        }

        public void AddOrUpdatePlayer(Player player)
        {
            Console.WriteLine("Checking PlayerID before update: " + player.PlayerID);
            var existingPlayerIndex = players.FindIndex(p => p.PlayerID == player.PlayerID);

            // If the same player (searched by playerID) is not found in the list, add the new player
            if (existingPlayerIndex == -1)
            {
                players.Add(player);
                Console.WriteLine("Added new player with ID: " + player.PlayerID);
            }
            else
            {
                var existingPlayer = players[existingPlayerIndex];
                existingPlayer.Username = player.Username; // Updating the username
                players[existingPlayerIndex] = existingPlayer; // Updating the player in the list
            }

            SaveLeaderboard();
        }
        
        public List<Player> GetLeaderboard()
        {
            return players.OrderByDescending(p => p.BestWPM).ToList();
        }
        
        private void SaveLeaderboard()
        {
            var options = new JsonSerializerOptions { WriteIndented = true }; // For better .json file readability
            var json = JsonSerializer.Serialize(players, options);
            File.WriteAllText(LeaderboardFile, json);
        }
        
        private List<Player> LoadLeaderboard()
        {
            try
            {
                if (!File.Exists(LeaderboardFile))
                {
                    return new List<Player>();
                }

                var json = File.ReadAllText(LeaderboardFile);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<List<Player>>(json, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading leaderboard: " + ex.Message);
                return new List<Player>();
            }
        }
    }
}