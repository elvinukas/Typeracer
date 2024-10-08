using Microsoft.AspNetCore.Mvc;
using Typeracer.Models;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Typeracer.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatisticsController : ControllerBase
{
    
    [HttpPost("save")]
    public IActionResult Save(StatisticsModel statisticsData)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            return BadRequest(new { message = "Invalid data.", errors });
        }

        if (statisticsData == null)
        {
            return BadRequest("Invalid data: statisticsData is null.");
        }

        // Calculating the WPM and accuracy for the entire paragraph
        TimeSpan completionTimeSpan = TimeSpan.FromMilliseconds(statisticsData.CompletionTime);
        double completionTimeInMinutes = completionTimeSpan.TotalMinutes;
        statisticsData.WordsPerMinute = CalculateWPM(statisticsData.TypedAmountOfWords, completionTimeInMinutes);
        int countedWordsSoFar = 0;
        double timeTakenSoFar = 0;

        foreach (var typingData in statisticsData.TypingData)
        {
            TimeSpan timeTakenForWord = typingData.EndingTimestampWord - typingData.BeginningTimestampWord;
            double timeTakenInMinutes = timeTakenForWord.TotalMinutes;
            ++countedWordsSoFar;
            timeTakenSoFar += timeTakenInMinutes;

            if (timeTakenInMinutes > 0)
            {
                typingData.CurrentWordsPerMinute = CalculateWPM(countedWordsSoFar, timeTakenSoFar);
            }
            else
            {
                // if a 1 letter word is the first word, its wpm is 0.
                if (countedWordsSoFar == 1 && timeTakenInMinutes == 0)
                {
                    typingData.CurrentWordsPerMinute = 0;
                }
                else
                {
                    // returning the previous wpm, since the results do not need to be disturbed by 1 letter word
                    typingData.CurrentWordsPerMinute =
                        CalculateWPM(countedWordsSoFar - 1, timeTakenSoFar - timeTakenInMinutes);
                }

            }
        }

        statisticsData.Accuracy = CalculateAccuracy(statisticsData.TotalAmountOfCharacters,
            statisticsData.NumberOfWrongfulCharacters);

        int typedCharsSoFar = 0;
        int amountOfMistakesSoFar = 0;

        foreach (var typingData in statisticsData.TypingData)
        {
            typedCharsSoFar += typingData.Word.Length;
            amountOfMistakesSoFar += typingData.AmountOfMistakesInWord;

            typingData.CurrentAccuracy = CalculateAccuracy(typedCharsSoFar, amountOfMistakesSoFar);
        }

        // Path to the statistics directory
        var statisticsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "statistics");

        // If the directory does not exist, creating it
        if (!Directory.Exists(statisticsDir))
        {
            Directory.CreateDirectory(statisticsDir);
        }

        // JSON file name and path
        var filePath = Path.Combine(statisticsDir, "game-data.json");

        // Converting the statistics data to JSON
        var json = JsonSerializer.Serialize(statisticsData, new JsonSerializerOptions 
        { 
            WriteIndented = true, 
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping // Special encoding option to prevent UTF-8 characters from being encoded
        });
        
        // Saving the JSON to a file
        System.IO.File.WriteAllText(filePath, json);

        Console.WriteLine($"Statistics saved to file: {filePath}");

        return Ok(new { message = "Statistics received and saved" });
    }

    private double CalculateWPM(int typedAmountOfWords, double completionTime)
    {
        if (completionTime != 0)
        {
            return typedAmountOfWords / completionTime;
        }
        else
        {
            return -1;
        }
    }

    private double CalculateAccuracy(int totalCharacters, int incorrectCharacters)
    {
        int correctCharacters = totalCharacters - incorrectCharacters;
        if (totalCharacters != 0)
        {
            return (double)correctCharacters / totalCharacters * 100;
        }
        else
        {
            return 0;
        }
    }
}