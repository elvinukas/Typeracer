namespace Typeracer.Models;

public class StatisticsInfoModel
{
    public TimeSpan CompletionTime { get; set; } // time that it took to finish the entire paragraph (or finish typing)
    public DateTime LocalStartTime { get; set; } // start date and time of the game
    public DateTime LocalFinishTime { get; set; } // end date and time of the game
    
    public int TotalAmountOfWords { get; set; } // amount of words in the paragraph
    public int TotalAmountOfCharacters { get; set; } // amount of characters in the paragraph;
    public int TypedAmountOfWords { get; set; }
    public int TypedAmountOfCharacters { get; set; }
    public int NumberOfWrongfulCharacters { get; set; }
    public List<TypingData> TypingData { get; set; } = new List<TypingData>(); // storing all data about the typed characters
    
}

public class TypingData
{
    public char Character { get; set; }
    public DateTime Timestamp { get; set; }
    public bool isCorrect { get; set; }
}