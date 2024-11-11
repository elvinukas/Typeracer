using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Typeracer.Context;

namespace Typeracer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    
    [HttpPost("save")]
    public IActionResult Save(StatisticsModel statisticsData, AppDbContext context)
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
        using (context)
        {
            context.Games.Add(game);
            context.SaveChanges();
        }
        
        // saving received statisticsData to a file
        
        // Path to the statistics directory
        // var statisticsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "statistics");
        //
        // // If the directory does not exist, creating it
        // if (!Directory.Exists(statisticsDir))
        // {
        //     Directory.CreateDirectory(statisticsDir);
        // }
        

        // JSON file name and path
        //var filePath = Path.Combine(statisticsDir, "game-data.json");
        
        //Console.WriteLine($"Statistics: {JsonSerializer.Serialize(game.Statistics, new JsonSerializerOptions { WriteIndented = true })}");

        // Converting the GAME DATA to JSON
        // var json = JsonSerializer.Serialize(game, new JsonSerializerOptions 
        // { 
        //     WriteIndented = true, 
        //     Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Special encoding option to prevent UTF-8 characters from being encoded
        // });
        //
        // // Saving the JSON to a file
        // System.IO.File.WriteAllText(filePath, json);
        //
        // Console.WriteLine($"Game information saved to file: {filePath}");

        return Ok(new { message = "Statistics received and game information saved to database" });
    }
    
}