using Microsoft.AspNetCore.Mvc;
using Typeracer.Context;
using Typeracer.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult GetGameById(string gameId)
        {
            Game? game = _context.Games
                .Include(g => g.Statistics)
                    .ThenInclude(s => s.TypingData)
                .Include(g => g.Statistics)
                    .ThenInclude(s => s.Paragraph)
                .FirstOrDefault(g => g.GameId == Guid.Parse(gameId));
            if (game == null)
            {
                return NotFound(new { message = "Game not found" });
            }
            return Ok(game);
        }
    }
}