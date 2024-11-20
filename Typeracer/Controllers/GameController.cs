using Microsoft.AspNetCore.Mvc;
using Typeracer.Context;
using Typeracer.Models;
using Microsoft.EntityFrameworkCore;
using Typeracer.Exceptions;

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
            Guid gameIdGuid = Guid.Parse(gameId);
            try
            {
                Game? game = GetGameById(gameIdGuid);
                return Ok(game);
            }
            catch (GameException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            
        }


        public Game GetGameById(Guid gameId)
        {
            Game? game = _context.Games
                .Include(g => g.Statistics)
                .ThenInclude(s => s.TypingData)
                .FirstOrDefault(g => g.GameId == gameId);

            if (game == null)
            {
                throw new GameException("Game not found");
            }

            return game;
        }
    }
}