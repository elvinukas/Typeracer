namespace Typeracer.Models;

public class StatisticsInfoModel
{
    private TimeSpan CompletionTime { get; set; } // time that it took to finish the entire paragraph (or finish typing)
    private DateTime LocalStartTime { get; set; } // start date and time of the game
    private DateTime LocalFinishTime { get; set; } // end date and time of the game
    
    private int TotalAmountOfWords { get; set; } // amount of words in the paragraph
    private int TotalAmountOfCharacters { get; set; } // amount of characters in the paragraph;
    private int TypedAmountOfWords { get; set; }
    private int TypedAmountOfCharacters { get; set; }
    private int NumberOfWrongfulCharacters { get; set; }
    public List<TypingData> TypingData { get; set; } = new List<TypingData>();
    
}

public class TypingData
{
    public char Character { get; set; }
    public DateTime Timestamp { get; set; }
    public bool isCorrect { get; set; }
}