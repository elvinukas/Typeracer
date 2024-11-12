using Microsoft.AspNetCore.Mvc;
using Typeracer.Context;
using Typeracer.Models;
using System.Linq;

namespace Typeracer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GameController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{gameId}")]
        public IActionResult GetGameById(Guid gameId)
        {
            Game? game = _context.Games.FirstOrDefault(g => g.GameId == gameId);
            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }
            return Ok(game);
        }
    }
}