using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;

namespace Typeracer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    
    [HttpPost("save")]
    // [FromBody] - deserializes JSON data to C# model
    // ./api/statistics/save
    public IActionResult Save([FromBody] StatisticsModel statisticsInfo)
    {
        if (statisticsInfo == null)
        {
            return BadRequest("Invalid data.");
        }
        
        // here the collected data should be proccessed
        
        // ----------------------------------------
        // CALCULATING WORDS PER MINUTE
        // ----------------------------------------
        
        // first completionTime must be converted to seconds
        double completionTimeInSeconds = statisticsInfo.CompletionTime.TotalSeconds;
        statisticsInfo.WordsPerMinute = statisticsInfo.TypedAmountOfWords / completionTimeInSeconds;
       // statisticsInfo.CorrectWordsPerMinute = statisticsInfo.
        
        // ----------------------------------------
        // CALCULATING ACCURACY (as a percentage)
        // ----------------------------------------
        
        //statisticsInfo.
        

        
        return Ok(new { message = "Statistics received and saved" });


    }
    
    
    
}