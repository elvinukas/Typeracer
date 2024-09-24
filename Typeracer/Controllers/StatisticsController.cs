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
        // its possible this will be changed/used for more advanced calculations of momentary wpm
        // ----------------------------------------
        
        // first completionTime must be converted to minutes
        double completionTimeInMinutes = statisticsInfo.CompletionTime.TotalMinutes;
        statisticsInfo.WordsPerMinute = CalculateWPM(statisticsInfo.TypedAmountOfWords, completionTimeInMinutes);
        
        
        // ----------------------------------------
        // CALCULATING ACCURACY (as a percentage)
        // ----------------------------------------
        
        //statisticsInfo.
        

        
        return Ok(new { message = "Statistics received and saved" });


    }

    private double CalculateWPM(int typedAmountOfWords, double completionTime)
    {
        return typedAmountOfWords / completionTime;
    }
    
    
    
}