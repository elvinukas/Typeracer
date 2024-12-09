using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Typeracer.Context;
using Typeracer.Exceptions;
using Typeracer.Models;
using Typeracer.Services;

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
            // fetch players along with their WPMs and Accuracies
            List<Player> players = _context.Players
                .Include(p => p.WPMs)
                .Include(p => p.Accuracies)
                .ToList();

            // initialize StatisticsAnalyzer instances (use of generic class)
            var wpmAnalyzer = new StatisticsAnalyzer<WPM>();
            var accuracyAnalyzer = new StatisticsAnalyzer<Accuracy>();

            // prepare the leaderboard data
            var leaderboard = players.Select(player =>
                {
                    double averageWPM = 0;
                    double averageAccuracy = 0;
                    double bestWPM = 0;
                    double bestAccuracy = 0;

                    // finding all statistical data with generic methods
                    if (player.WPMs != null && player.WPMs.Count > 0)
                    {
                        averageWPM = wpmAnalyzer.CalculateAverage(player.WPMs, wpm => wpm.Value);
                        bestWPM = wpmAnalyzer.FindBestItem(player.WPMs).Value;
                    }

                    if (player.Accuracies != null && player.Accuracies.Count > 0)
                    {
                        averageAccuracy = accuracyAnalyzer.CalculateAverage(player.Accuracies, acc => acc.Value);
                        bestAccuracy = accuracyAnalyzer.FindBestItem(player.Accuracies).Value;
                    }

                    return new
                    {
                        player.PlayerID,
                        player.Username,
                        BestWPM = bestWPM,
                        BestAccuracy = bestAccuracy,
                        AverageWPM = averageWPM,
                        AverageAccuracy = averageAccuracy
                    };
                })
                .OrderByDescending(p => p.BestWPM)
                .Take(10)
                .ToList();

            return Ok(leaderboard);
        }
        
        [HttpPost("save")]
        public IActionResult SavePlayerData([FromBody] PlayerDataModel playerData, AppDbContext context)
        {
            if (string.IsNullOrEmpty(playerData.Username))
            {
                return BadRequest("Username is not set!");
            }

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
                    Player? existingPlayer = context.Players
                        .Include(p => p.WPMs)
                        .Include(p => p.Accuracies)
                        .FirstOrDefault(p => p.Username == player.Username);
                    if (existingPlayer == null)
                    {
                        context.Players.Add(player);
                        context.SaveChanges();
                    }
                    else
                    {
                        player = existingPlayer;
                        WPM newWPM = new WPM
                        {
                            Value = playerData.BestWPM, PlayerId = existingPlayer.PlayerID
                        };
        
                        Accuracy newAccuracy = new Accuracy
                        {
                            Value = playerData.BestAccuracy, PlayerId = existingPlayer.PlayerID
                        };


                        context.Wpms.Add(newWPM);
                        context.Accuracies.Add(newAccuracy);
                        context.SaveChanges();
                    }


                    GameController gameController = new GameController(context);
                    Game? game;
                    try
                    {
                        game = gameController.GetGameById(Guid.Parse(playerData.GameId));
                    }
                    catch (GameException ex)
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