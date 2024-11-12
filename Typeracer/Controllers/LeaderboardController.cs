using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Typeracer.Context;
using Typeracer.Models;
using System.Transactions;

namespace Typeracer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly Leaderboard _leaderboard;

        public LeaderboardController(Leaderboard leaderboard)
        {
            _leaderboard = leaderboard;
        }

        [HttpGet]
        public IActionResult GetLeaderboard()
        {
            return Ok(_leaderboard.GetLeaderboard());
        }

        [HttpPost("save")]
        public IActionResult SavePlayerData([FromBody] PlayerDataModel playerData, AppDbContext context)
        {
            if (string.IsNullOrEmpty(playerData.Username))
            {
                return BadRequest("Vartotojo vardas būtinas");
            }

            // if (string.IsNullOrEmpty(playerData.PlayerID) || !Guid.TryParse(playerData.PlayerID, out Guid playerGuid))
            // {
            //     return BadRequest("Neteisingas PlayerID formatas");
            // }

            //Console.WriteLine($"Received PlayerID: {playerData.PlayerID}");
            Console.WriteLine($"Received Username: {playerData.Username}");
            Console.WriteLine($"GameID: {playerData.GameId}");

            var player = new Player(
                playerData.Username,
                playerData.BestWPM,
                playerData.BestAccuracy
            );

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Player? existingPlayer = context.Players.Find(player.PlayerID);
                    if (existingPlayer == null)
                    {
                        context.Players.Add(player);
                        context.SaveChanges();
                    }
                    

                    Game? game = context.Games.FirstOrDefault(g => g.GameId == playerData.GameId);
                    if (game == null)
                    {
                        return NotFound("Specified game not found.");
                    }

                    if (game.PlayerId != null)
                    {
                        return BadRequest("Game already has a player attached!");
                    }
                    game.PlayerId = player.PlayerID;
                    context.Update(game);
                    context.SaveChanges();

                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
}
            

            _leaderboard.AddOrUpdatePlayer(player);

            return Ok("Player added to game successfully!");
        }
    }
}