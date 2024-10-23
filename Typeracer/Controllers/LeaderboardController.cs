using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;

namespace Typeracer.Controllers
{
    [Route("api/leaderboard")]
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
        public IActionResult SavePlayerData([FromBody] PlayerDataModel playerData)
        {
            if (string.IsNullOrEmpty(playerData.Username))
            {
                return BadRequest("Vartotojo vardas būtinas");
            }

            if (string.IsNullOrEmpty(playerData.PlayerID) || !Guid.TryParse(playerData.PlayerID, out Guid playerGuid))
            {
                return BadRequest("Neteisingas PlayerID formatas");
            }

            Console.WriteLine($"Received PlayerID: {playerData.PlayerID}");
            Console.WriteLine($"Received Username: {playerData.Username}");

            var player = new Player(
                playerData.Username,
                playerGuid,
                playerData.BestWPM,
                playerData.BestAccuracy
            );

            _leaderboard.AddOrUpdatePlayer(player);

            return Ok();
        }
    }
}