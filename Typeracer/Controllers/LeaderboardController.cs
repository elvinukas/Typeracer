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
        private readonly AppDbContext _context;

        public LeaderboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetLeaderboard()
        {
            List<Player> leaderboard = _context.Players
                .Include(p => p.WPMs)
                .Include(p => p.Accuracies)
                .OrderByDescending(p => p.WPMs.Max(w => w.Value))
                .ToList();

            return Ok(leaderboard);
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


                    GameController gameController = new GameController(context);
                    Game? game = gameController.GetGameById(Guid.Parse(playerData.GameId));
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
            
            return Ok("Player added to game successfully!");
        }
        
        
        
    }
}