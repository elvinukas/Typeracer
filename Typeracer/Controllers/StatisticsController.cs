using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;
using Typeracer.Context;

namespace Typeracer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    public readonly AppDbContext _DbContext;

    public StatisticsController(AppDbContext context)
    {
        _DbContext = context;
    }

    [HttpGet("{statisticsId}")]
    public IActionResult GetStatistics(string statisticsId)
    {
        StatisticsModel? statisticsModel = _DbContext.Statistics
            .FirstOrDefault(s => s.StatisticsId == Guid.Parse(statisticsId));

        if (statisticsModel == null)
        {
            return NotFound("Such statistics don't exist.");
        }

        return Ok(statisticsModel);
    }
    
    
    [HttpPost("save")]
    public IActionResult Save(StatisticsModel statisticsData)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors) // LINQ
                .Select(e => e.ErrorMessage) // LINQ
                .ToList(); // LINQ
            return BadRequest(new { message = "Invalid data.", errors });
        }

        if (statisticsData == null)
        {
            return BadRequest("Invalid data: statisticsData is null.");
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
        using (_DbContext)
        {
            _DbContext.Games.Add(game);
            _DbContext.SaveChanges();
        }
        
        return Ok(new { message = "Statistics received and game information saved to database", gameId = game.GameId });
    }
    
}