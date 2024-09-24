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
        
        // first completionTime must be converted to minutes
        double completionTimeInMinutes = statisticsInfo.CompletionTime.TotalMinutes;
        statisticsInfo.WordsPerMinute = CalculateWPM(statisticsInfo.TypedAmountOfWords, completionTimeInMinutes);
        
        
        // calculating momentary wpm for each typingdata element
        foreach (var typingData in statisticsInfo.TypingData)
        {
            TimeSpan timeTakenForWord = typingData.EndingTimestampWord - typingData.BeginningTimestampWord;
            double timeTakenInMinutes = timeTakenForWord.TotalMinutes;
            
            // calculate and store wpm for each word typed

            if (timeTakenInMinutes > 0)
            {
                typingData.MomentaryWordsPerMinute = CalculateWPM(1, timeTakenInMinutes);
            }
            else
            {
                typingData.MomentaryWordsPerMinute = -1; // -1 is indicating that the time spent
                                                         // to write a word was instant
            }
        }
        
        // ----------------------------------------
        // CALCULATING ACCURACY (as a percentage)
        // ----------------------------------------

        statisticsInfo.Accuracy = CalculateAccuracy(statisticsInfo.TotalAmountOfCharacters,
            statisticsInfo.NumberOfWrongfulCharacters);
        
        
        // calculating momentary accuracy for each typingData element;

        foreach (var typingData in statisticsInfo.TypingData)
        {
            typingData.MomentaryAccuracy = CalculateAccuracy(typingData.Word.Length, 
                typingData.amountOfMistakesInWord);
        }
        
        return Ok(new { message = "Statistics received and saved" });


    }

    private double CalculateWPM(int typedAmountOfWords, double completionTime)
    {
        return typedAmountOfWords / completionTime;
    }

    private double CalculateAccuracy(int totalCharacters, int incorrectCharacters)
    {
        int correctCharacters = totalCharacters - incorrectCharacters;
        if (totalCharacters != 0)
        {
            return (correctCharacters - incorrectCharacters) / totalCharacters * 100;
        } else
        {
            return 0;
        }
    }



}