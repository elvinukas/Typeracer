using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Typeracer.Models;
using Typeracer.Context;

namespace Typeracer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StatisticsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{statisticsId}")]
    public IActionResult GetStatistics(string statisticsId)
    {
        StatisticsModel? statisticsModel = _context.Statistics
            .FirstOrDefault(s => s.StatisticsId == Guid.Parse(statisticsId));

        if (statisticsModel == null)
        {
            return NotFound("Such statistics don't exist.");
        }

        return Ok(statisticsModel);
    }

    [HttpPost]
    public async Task<IActionResult> SaveStatistics([FromBody] StatisticsModel statisticsData)
    {
        if (statisticsData == null)
        {
            return BadRequest("Invalid data: statisticsData is null.");
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors) // LINQ
                .Select(e => e.ErrorMessage) // LINQ
                .ToList(); // LINQ
            return BadRequest(new { message = "Invalid data.", errors });
        }

        statisticsData.LocalStartTime = DateTime.SpecifyKind(statisticsData.LocalStartTime.Value, DateTimeKind.Utc);
        statisticsData.LocalFinishTime = DateTime.SpecifyKind(statisticsData.LocalFinishTime.Value, DateTimeKind.Utc);

        foreach (TypingData data in statisticsData.TypingData)
        {
            data.BeginningTimestampWord = DateTime.SpecifyKind(data.BeginningTimestampWord, DateTimeKind.Utc);
            data.EndingTimestampWord = DateTime.SpecifyKind(data.EndingTimestampWord, DateTimeKind.Utc);
        }
        
        // initiating a game object with all the statistics data
        Game game = new Game(statisticsData);
        _context.Games.Add(game);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, new { message = "An error occurred while saving the game data.", error = ex.Message });
        }

        return Ok(new { message = "Statistics received and game information saved to database", gameId = game.GameId });
    }
}